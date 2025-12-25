using System.Device.Gpio;
using System.Device.Spi;

namespace ElectrcFox.FennecSdr.Touch;

public class Xpt2046 : IDisposable
{
    public event Action<TouchEvent>? TouchEventReceived;

    private readonly SpiDevice _spi;
    private readonly GpioController _gpio;
    private readonly TouchCalibration _touchCalibration;
    private readonly int _irqPinNumber;
    private readonly object _spiLock;

    private readonly SemaphoreSlim _touchEventQueue = new(0);

    private Task? _workerTask;
    private readonly CancellationTokenSource _cts = new();

    private bool _isTouching = false;
    private TouchPoint _lastPoint;
    private TouchPoint _stablePoint;
    private int _stableCount = 0;
    private DateTime _lastTouchTime;
    private bool disposedValue;

    // Thresholds for debounce and movement
    private const int MoveThresholdPx = 4;
    private const int StableCountRequired = 3;
    private const int ReleaseTimeoutMs = 50;

    public Xpt2046(
        int spiBusId,
        int csPin,
        int irqPin,
        TouchCalibration touchCalibration,
        object spiLock
    )
    {
        _touchCalibration = touchCalibration;

        var settings = new SpiConnectionSettings(spiBusId, csPin)
        {
            Mode = SpiMode.Mode0,
            ClockFrequency = 2_000_000,
        };

        _spi = SpiDevice.Create(settings);

        _irqPinNumber = irqPin;

        _gpio = new GpioController();
        _spiLock = spiLock;
    }

    public void Start()
    {
        _gpio.OpenPin(_irqPinNumber, PinMode.InputPullUp);
        _gpio.RegisterCallbackForPinValueChangedEvent(
            _irqPinNumber,
            PinEventTypes.Rising | PinEventTypes.Falling,
            IrqPinValueChanged
        );

        _workerTask = Task.Run(RunAsync);
    }

    private async Task RunAsync()
    {
        while (_cts.IsCancellationRequested == false)
        {
            await _touchEventQueue.WaitAsync(_cts.Token);

            int rawX,
                rawY,
                pressure;

            lock (_spiLock)
            {
                if (!TryReadRaw(out rawX, out rawY, out pressure))
                {
                    continue;
                }
            }

            ProcessRawSample(rawX, rawY, pressure);
        }
    }

    private void IrqPinValueChanged(object sender, PinValueChangedEventArgs args)
    {
        _touchEventQueue.Release();
    }

    private bool TryReadRaw(out int rawX, out int rawY, out int pressure)
    {
        rawX = rawY = pressure = 0;

        int Read12Bit(byte command)
        {
            Span<byte> tx = stackalloc byte[3];
            Span<byte> rx = stackalloc byte[3];

            tx[0] = command;
            tx[1] = 0x00;
            tx[2] = 0x00;

            _spi.TransferFullDuplex(tx, rx);

            return ((rx[1] << 8) | rx[2]) >> 3;
        }

        // Order matters: Y first, then X
        int y = Read12Bit(0x90);
        int x = Read12Bit(0xD0);

        // Optional pressure
        int z1 = Read12Bit(0xB0);
        int z2 = Read12Bit(0xC0);
        int z = z1 + 4095 - z2;

        // Basic validity checks
        if (x <= 0 || y <= 0 || x >= 4095 || y >= 4095)
            return false;

        rawX = x;
        rawY = y;
        pressure = z;

        return true;
    }

    // Read raw 12-bit X/Y coordinate
    private int ReadRaw(byte command)
    {
        Span<byte> write = stackalloc byte[3];
        Span<byte> read = stackalloc byte[3];

        write[0] = command;
        write[1] = 0;
        write[2] = 0;

        _spi.TransferFullDuplex(write, read);

        int value = (read[1] << 8 | read[2]) >> 3; // 12-bit
        return value;
    }

    private void ProcessRawSample(int rawX, int rawY, int pressure)
    {
        var now = DateTime.UtcNow;

        if (pressure <= 0)
        {
            HandleRelease(now);
            return;
        }

        var point = Calibrate(rawX, rawY);
        _lastTouchTime = now;

        if (!_isTouching)
        {
            BeginTouch(point);
            return;
        }

        HandleMove(point);
    }

    private void HandleRelease(DateTime now)
    {
        if (!_isTouching)
        {
            return;
        }

        if ((now - _lastTouchTime).TotalMilliseconds < ReleaseTimeoutMs)
        {
            return;
        }

        _isTouching = false;

        TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Up, _lastPoint));
    }

    private void BeginTouch(TouchPoint point)
    {
        _stablePoint = point;
        _stableCount = 1;

        if (_stableCount >= StableCountRequired)
        {
            _isTouching = true;
            _lastPoint = _stablePoint;
            TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Down, _lastPoint));
        }
    }

    private void HandleMove(TouchPoint point)
    {
        int dx = Math.Abs(point.X - _lastPoint.X);
        int dy = Math.Abs(point.Y - _lastPoint.Y);

        if (dx < MoveThresholdPx && dy < MoveThresholdPx)
        {
            return;
        }

        _lastPoint = point;

        TouchEventReceived?.Invoke(new TouchEvent(TouchEventType.Move, point));
    }

    private TouchPoint Calibrate(int rawX, int rawY)
    {
        if (_touchCalibration.SwapXY)
        {
            (rawX, rawY) = (rawY, rawX);
        }

        if (_touchCalibration.InvertX)
        {
            rawX = _touchCalibration.MaxX - (rawX - _touchCalibration.MinX);
        }

        if (_touchCalibration.InvertY)
        {
            rawY = _touchCalibration.MaxY - (rawY - _touchCalibration.MinY);
        }

        int x =
            (rawX - _touchCalibration.MinX)
            * 320
            / (_touchCalibration.MaxX - _touchCalibration.MinX);

        int y =
            (rawY - _touchCalibration.MinY)
            * 240
            / (_touchCalibration.MaxY - _touchCalibration.MinY);

        x = Math.Clamp(x, 0, 319);
        y = Math.Clamp(y, 0, 239);

        return new TouchPoint(x, y);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _cts.Cancel();
                _workerTask?.Wait();

                _gpio.UnregisterCallbackForPinValueChangedEvent(
                    _irqPinNumber,
                    IrqPinValueChanged);

                _gpio.ClosePin(_irqPinNumber);
                _touchEventQueue.Dispose();
                _cts.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

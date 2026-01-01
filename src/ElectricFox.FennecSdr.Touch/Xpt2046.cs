using ElectricFox.EmbeddedApplicationFramework.Touch;
using System.Device.Gpio;
using System.Device.Spi;

namespace ElectricFox.FennecSdr.Touch;

public class Xpt2046 : IDisposable, ITouchController
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

    private bool _disposedValue;
    private bool _spiActive;

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
            ClockFrequency = 1_000_000,
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
            PinEventTypes.Falling,
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
        if (_spiActive)
        {
            return;
        }

        Console.WriteLine("IRQ fired");
        _touchEventQueue.Release();
    }

    private bool TryReadRaw(out int rawX, out int rawY, out int pressure)
    {
        rawX = rawY = pressure = 0;

        _spiActive = true;

        try
        {
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

            Console.WriteLine($"RAW X={x} Y={y} Z={z}");

            return true;
        }
        finally
        {
            _spiActive = false;
        }
    }

    private void ProcessRawSample(int rawX, int rawY, int pressure)
    {
        var now = DateTime.UtcNow;

        var point = Calibrate(rawX, rawY);

        Console.WriteLine($"Calibrated to X={point.X} Y={point.Y}, P={pressure}");

        TouchEventReceived?.Invoke(new TouchEvent(point));
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
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cts.Cancel();
                _workerTask?.Wait();

                _gpio.UnregisterCallbackForPinValueChangedEvent(_irqPinNumber, IrqPinValueChanged);

                _gpio.ClosePin(_irqPinNumber);
                _touchEventQueue.Dispose();
                _cts.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

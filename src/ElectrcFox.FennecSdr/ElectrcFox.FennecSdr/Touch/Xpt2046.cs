using System.Device.Gpio;
using System.Device.Spi;

namespace ElectrcFox.FennecSdr.Touch;

public class Xpt2046
{
    private readonly SpiDevice _spi;
    private readonly GpioController _gpio;
    private readonly int _irqPin;
    private readonly TouchCalibration _touchCalibration;

    private const byte CMD_X = 0b10010000;
    private const byte CMD_Y = 0b11010000;

    private int minX = 474, minY = 332, maxX = 3352, maxY = 3900;

    public Xpt2046(int spiBusId, int csPin, int irqPin, TouchCalibration touchCalibration)
    {
        _touchCalibration = touchCalibration;

        var settings = new SpiConnectionSettings(spiBusId, csPin)
        {
            Mode = SpiMode.Mode0,
            ClockFrequency = 2_000_000
        };

        _spi = SpiDevice.Create(settings);

        this._irqPin = irqPin;

        _gpio = new GpioController();
        _gpio.OpenPin(irqPin, PinMode.InputPullUp);
    }

    public bool IsPressed() => _gpio.Read(_irqPin) == PinValue.Low;

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

    public (int x, int y)? GetTouch()
    {
        if (!IsPressed())
        {
            return null;
        }

        // Command bits: see XPT2046 datasheet
        int rawX = ReadRaw(CMD_X);
        int rawY = ReadRaw(CMD_Y);

        Console.WriteLine($"Raw Touch Values: {rawX}, {rawY}");

        if (rawX < minX || rawX > maxX || rawY < minY || rawY > maxY)
        {
            // Update bounds
            minX = Math.Min(minX, rawX);
            minY = Math.Min(minY, rawY);
            maxX = Math.Max(maxX, rawX);
            maxY = Math.Max(maxY, rawY);
            Console.WriteLine($"NEW BOUNDS: {minX}, {minY} to {maxX}, {maxY}");
        }

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

        int x = (rawX - _touchCalibration.MinX) * 320 / (_touchCalibration.MaxX - _touchCalibration.MinX);
        int y = (rawY - _touchCalibration.MinY) * 240 / (_touchCalibration.MaxY - _touchCalibration.MinY);

        x = Math.Clamp(x, 0, 319);
        y = Math.Clamp(y, 0, 239);

        return (x, y);
    }
}

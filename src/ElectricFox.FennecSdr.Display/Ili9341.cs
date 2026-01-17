using System.Device.Gpio;
using System.Device.Spi;

namespace ElectricFox.FennecSdr.Display;

public class Ili9341
{
    private const int DcPin = 25;
    private const int ResetPin = 24;

    private readonly SpiDevice _spi;
    private readonly GpioController _gpio;

    private readonly object _spiLock;

    public Ili9341(int spiBusId, int csPin, object spiLock)
    {
        _gpio = new GpioController();
        _gpio.OpenPin(DcPin, PinMode.Output);
        _gpio.OpenPin(ResetPin, PinMode.Output);

        var settings = new SpiConnectionSettings(spiBusId, csPin)
        {
            ClockFrequency = 16_000_000,
            Mode = SpiMode.Mode0
        };

        _spi = SpiDevice.Create(settings);
        _spiLock = spiLock;
    }
    
    public void BeginWrite()
    {
        WriteCommand(0x2C);

        lock (_spiLock)
        {
            _gpio.Write(DcPin, PinValue.High);
        }
    }

    public void WriteScanline(ReadOnlySpan<byte> rgb565Line)
    {
        lock (_spiLock)
        {
            _spi.Write(rgb565Line);
        }
    }

    void WriteCommand(byte cmd)
    {
        lock (_spiLock)
        {
            _gpio.Write(DcPin, PinValue.Low);
            _spi.WriteByte(cmd);
        }
    }

    void WriteData(ReadOnlySpan<byte> data)
    {
        lock (_spiLock)
        {
            _gpio.Write(DcPin, PinValue.High);
            _spi.Write(data);
        }
    }

    public void Reset()
    {
        lock (_spiLock)
        {
            _gpio.Write(ResetPin, PinValue.Low);
        }

        Thread.Sleep(20);

        lock (_spiLock)
        {
            _gpio.Write(ResetPin, PinValue.High);
        }

        Thread.Sleep(150);
    }

    public void Init()
    {
        Reset();

        WriteCommand(0x01); // Software reset
        Thread.Sleep(150);

        WriteCommand(0x28); // Display OFF

        WriteCommand(0x3A); // Pixel format
        WriteData([0x55]); // 16-bit

        WriteCommand(0x36); // Memory Access Control
        WriteData([0x28]); // Landscape, RGB

        WriteCommand(0x11); // Sleep OUT
        Thread.Sleep(120);

        WriteCommand(0x29); // Display ON
    }
    
    public void SetAddressWindow(int x, int y, int w, int h)
    {
        WriteCommand(0x2A);
        WriteData(stackalloc byte[]
        {
            (byte)(x >> 8), (byte)x,
            (byte)((x + w - 1) >> 8), (byte)(x + w - 1)
        });

        WriteCommand(0x2B);
        WriteData(stackalloc byte[]
        {
            (byte)(y >> 8), (byte)y,
            (byte)((y + h - 1) >> 8), (byte)(y + h - 1)
        });
    }
}
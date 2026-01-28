using System.Device.Gpio;
using System.Device.Spi;
using ElectricFox.EmbeddedApplicationFramework.Display;

namespace ElectricFox.FennecSdr.Display;

/// <summary>
/// ILI9488 TFT LCD driver (320x480, RGB666/RGB565)
/// </summary>
public class Ili9488 : ILcdDevice
{
    private const int DcPin = 25;
    private const int ResetPin = 24;

    private readonly SpiDevice _spi;
    private readonly GpioController _gpio;
    private readonly object _spiLock;
    private readonly IPixelConverter _pixelConverter;
    private readonly bool _useRgb565;

    public int Width => 320;
    public int Height => 480;
    public IPixelConverter PixelConverter => _pixelConverter;

    /// <summary>
    /// Creates a new ILI9488 driver
    /// </summary>
    /// <param name="spiBusId">SPI bus ID</param>
    /// <param name="csPin">Chip select pin</param>
    /// <param name="spiLock">Lock object for SPI synchronization</param>
    /// <param name="useRgb565">If true, uses RGB565 (2 bytes/pixel). If false, uses RGB666 (3 bytes/pixel)</param>
    public Ili9488(int spiBusId, int csPin, object spiLock, bool useRgb565 = false)
    {
        _gpio = new GpioController();
        _gpio.OpenPin(DcPin, PinMode.Output);
        _gpio.OpenPin(ResetPin, PinMode.Output);

        var settings = new SpiConnectionSettings(spiBusId, csPin)
        {
            ClockFrequency = 16_000_000, // ILI9488 supports up to 20MHz
            Mode = SpiMode.Mode0
        };

        _spi = SpiDevice.Create(settings);
        _spiLock = spiLock;
        _useRgb565 = useRgb565;
        
        // Device provides its own converter based on configuration
        _pixelConverter = useRgb565 
            ? new Rgb565PixelConverter() 
            : new Rgb666PixelConverter();
    }
    
    public void BeginWrite()
    {
        WriteCommand(0x2C); // Memory Write

        lock (_spiLock)
        {
            _gpio.Write(DcPin, PinValue.High);
        }
    }

    /// <summary>
    /// Write a scanline. Data should be in RGB565 (2 bytes) or RGB666 (3 bytes) format depending on initialization
    /// </summary>
    public void WriteScanline(ReadOnlySpan<byte> scanlineData)
    {
        lock (_spiLock)
        {
            _spi.Write(scanlineData);
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

        // Positive Gamma Control
        WriteCommand(0xE0);
        WriteData([0x00, 0x03, 0x09, 0x08, 0x16, 0x0A, 0x3F, 0x78, 0x4C, 0x09, 0x0A, 0x08, 0x16, 0x1A, 0x0F]);

        // Negative Gamma Control
        WriteCommand(0xE1);
        WriteData([0x00, 0x16, 0x19, 0x03, 0x0F, 0x05, 0x32, 0x45, 0x46, 0x04, 0x0E, 0x0D, 0x35, 0x37, 0x0F]);

        // Power Control 1
        WriteCommand(0xC0);
        WriteData([0x17, 0x15]);

        // Power Control 2
        WriteCommand(0xC1);
        WriteData([0x41]);

        // VCOM Control
        WriteCommand(0xC5);
        WriteData([0x00, 0x12, 0x80]);

        // Memory Access Control (rotation and color order)
        WriteCommand(0x36);
        WriteData([0x48]); // MX, BGR (landscape mode)

        // Interface Pixel Format
        WriteCommand(0x3A);
        if (_useRgb565)
        {
            WriteData([0x55]); // 16-bit RGB565
        }
        else
        {
            WriteData([0x66]); // 18-bit RGB666
        }

        // Frame Rate Control
        WriteCommand(0xB1);
        WriteData([0xA0]); // 60Hz

        // Display Inversion Control
        WriteCommand(0xB4);
        WriteData([0x02]); // 2-dot inversion

        // Display Function Control
        WriteCommand(0xB6);
        WriteData([0x02, 0x02]); // Non-display area source output level

        // Entry Mode Set
        WriteCommand(0xB7);
        WriteData([0xC6]);

        // Sleep OUT
        WriteCommand(0x11);
        Thread.Sleep(120);

        // Display ON
        WriteCommand(0x29);
        Thread.Sleep(25);
    }
    
    /// <summary>
    /// Set the drawing window on the display
    /// </summary>
    /// <param name="x">X coordinate (0-319)</param>
    /// <param name="y">Y coordinate (0-479)</param>
    /// <param name="w">Width in pixels</param>
    /// <param name="h">Height in pixels</param>
    public void SetAddressWindow(int x, int y, int w, int h)
    {
        // Column Address Set
        WriteCommand(0x2A);
        WriteData(stackalloc byte[]
        {
            (byte)(x >> 8), (byte)x,
            (byte)((x + w - 1) >> 8), (byte)(x + w - 1)
        });

        // Page Address Set
        WriteCommand(0x2B);
        WriteData(stackalloc byte[]
        {
            (byte)(y >> 8), (byte)y,
            (byte)((y + h - 1) >> 8), (byte)(y + h - 1)
        });
    }

    /// <summary>
    /// Set display brightness (if supported)
    /// </summary>
    /// <param name="brightness">Brightness level 0-255</param>
    public void SetBrightness(byte brightness)
    {
        WriteCommand(0x51); // Write Display Brightness
        WriteData([brightness]);
    }

    /// <summary>
    /// Enable/disable display inversion
    /// </summary>
    public void SetInversion(bool invert)
    {
        WriteCommand(invert ? (byte)0x21 : (byte)0x20);
    }
}
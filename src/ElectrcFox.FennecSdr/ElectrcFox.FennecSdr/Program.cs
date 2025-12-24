using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

class Ili9341
{
    const int DcPin = 25;
    const int ResetPin = 24;

    private SpiDevice spi;
    private GpioController gpio;

    public Ili9341()
    {
        gpio = new GpioController();
        gpio.OpenPin(DcPin, PinMode.Output);
        gpio.OpenPin(ResetPin, PinMode.Output);

        var settings = new SpiConnectionSettings(0, 0)
        {
            ClockFrequency = 16_000_000,
            Mode = SpiMode.Mode0
        };

        spi = SpiDevice.Create(settings);
    }

    void WriteCommand(byte cmd)
    {
        gpio.Write(DcPin, PinValue.Low);
        spi.WriteByte(cmd);
    }

    void WriteData(ReadOnlySpan<byte> data)
    {
        gpio.Write(DcPin, PinValue.High);
        spi.Write(data);
    }

    public void Reset()
    {
        gpio.Write(ResetPin, PinValue.Low);
        Thread.Sleep(20);
        gpio.Write(ResetPin, PinValue.High);
        Thread.Sleep(150);
    }

    public void Init()
    {
        Reset();

        WriteCommand(0x01); // Software reset
        Thread.Sleep(150);

        WriteCommand(0x28); // Display OFF

        WriteCommand(0x3A); // Pixel format
        WriteData(stackalloc byte[] { 0x55 }); // 16-bit

        WriteCommand(0x36); // Memory Access Control
        WriteData(stackalloc byte[] { 0x28 }); // Landscape, RGB

        WriteCommand(0x11); // Sleep OUT
        Thread.Sleep(120);

        WriteCommand(0x29); // Display ON
    }

    public void FillScreen(ushort color)
    {
        const int Width = 320;
        const int Height = 240;

        WriteCommand(0x2A); // Column addr
        WriteData(stackalloc byte[]
        {
            0x00, 0x00,
            (byte)((Width - 1) >> 8),
            (byte)((Width - 1) & 0xFF)
        });

        WriteCommand(0x2B); // Page addr
        WriteData(stackalloc byte[]
        {
            0x00, 0x00,
            (byte)((Height - 1) >> 8),
            (byte)((Height - 1) & 0xFF)
        });

        WriteCommand(0x2C); // Memory write

        Span<byte> line = stackalloc byte[Width * 2];
        for (int i = 0; i < line.Length; i += 2)
        {
            line[i] = (byte)(color >> 8);
            line[i + 1] = (byte)(color & 0xFF);
        }

        gpio.Write(DcPin, PinValue.High);
        for (int y = 0; y < Height; y++)
            spi.Write(line);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("ILI9341 test");

        var lcd = new Ili9341();
        lcd.Init();

        lcd.FillScreen(0xF800); // RED
        Thread.Sleep(1000);
        lcd.FillScreen(0x07E0); // GREEN
        Thread.Sleep(1000);
        lcd.FillScreen(0x001F); // BLUE

        Console.WriteLine("Done");
    }
}
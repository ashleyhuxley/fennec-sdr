using System.Device.Gpio;
using System.Device.Spi;

namespace ElectrcFox.FennecSdr;

public class Xpt2046
{
    private readonly SpiDevice spi;
    private readonly GpioController gpio;
    private readonly int irqPin;

    public Xpt2046(int spiBusId, int csPin, int irqPin)
    {
        var settings = new SpiConnectionSettings(spiBusId, csPin)
        {
            Mode = SpiMode.Mode0,
            ClockFrequency = 2_000_000
        };

        spi = SpiDevice.Create(settings);

        this.irqPin = irqPin;

        gpio = new GpioController();
        gpio.OpenPin(irqPin, PinMode.InputPullUp);
    }

    public bool IsPressed() => gpio.Read(irqPin) == PinValue.Low;

    // Read raw 12-bit X/Y coordinate
    private int ReadRaw(byte command)
    {
        Span<byte> write = stackalloc byte[3];
        Span<byte> read = stackalloc byte[3];

        write[0] = command;
        write[1] = 0;
        write[2] = 0;

        spi.TransferFullDuplex(write, read);

        int value = ((read[1] << 8) | read[2]) >> 3; // 12-bit
        return value;
    }

    public (int x, int y)? GetTouch()
    {
        if (!IsPressed())
            return null;

        // Command bits: see XPT2046 datasheet
        const byte CMD_X = 0b10010000; // differential X
        const byte CMD_Y = 0b11010000; // differential Y

        int x = ReadRaw(CMD_X);
        int y = ReadRaw(CMD_Y);

        Console.WriteLine($"Raw Touch Values: {x}, {y}");

        // Map raw 0–4095 to screen 0–319 / 0–239
        int px = x * 320 / 4096;
        int py = y * 240 / 4096;

        return (px, py);
    }
}

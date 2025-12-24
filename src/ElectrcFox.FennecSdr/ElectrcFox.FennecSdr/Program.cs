using SixLabors.ImageSharp;
using System;
using System.Device.Gpio;
using System.Device.Spi;
using System.Threading;

namespace ElectrcFox.FennecSdr;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("ILI9341 test");

        var lcd = new Ili9341();
        lcd.Init();

        var gfx = new GraphicsRenderer(lcd);

        gfx.Clear(Color.Black);
        gfx.DrawText("SDR Device", 80, 20, Color.White);
        gfx.DrawRect(10, 60, 300, 160, Color.Green);
        gfx.DrawText("Tone Finder", 20, 80, Color.Yellow);
        gfx.DrawText("Spectrum", 20, 120, Color.Cyan);

        gfx.Flush();

        Console.WriteLine("Done");
    }
}
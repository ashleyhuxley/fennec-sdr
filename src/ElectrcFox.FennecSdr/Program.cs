using ElectrcFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.Display;
using SixLabors.ImageSharp;

namespace ElectrcFox.FennecSdr;

public class Program
{
    public static async Task Main(string[] args)
    {
        var touchCal = new TouchCalibration(369, 3538, 332, 3900, true, false, true);
        var spiLock = new object();

        var lcd = new Ili9341(0, 0, spiLock);
        var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 17, touchCal, spiLock);

        touch.TouchEventReceived += (te) =>
        {
            Console.WriteLine($"Touch Event: {te.Type} at {te.Point.X},{te.Point.Y}");
        };

        touch.Start();
        lcd.Init();

        var gfx = new GraphicsRenderer(new LcdScanlineTarget(lcd, 320, 240));
        gfx.Clear(Color.Black);
        gfx.DrawText("Fennec SDR", 20, 20, Color.White);
        gfx.Flush();

        Console.WriteLine("Fennec SDR Ready");

        while (true)
        {
            await Task.Delay(10);
        }
    }
}
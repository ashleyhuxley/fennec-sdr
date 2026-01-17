using ElectricFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.Display;
using ElectricFox.FennecSdr.App;

namespace ElectricFox.FennecSdr;

public class Program
{
    public static async Task Main(string[] args)
    {
        var touchCal = new TouchCalibration(369, 3538, 332, 3900, true, true, false);
        var spiLock = new object();

        var lcd = new Ili9341(0, 0, spiLock);
        var touch = new Xpt2046(spiBusId: 0, csPin: 1, irqPin: 17, touchCal, spiLock);

        touch.TouchEventReceived += (te) =>
        {
            Console.WriteLine($"Touch Event: {te.Point.X},{te.Point.Y}");
        };

        touch.Start();
        lcd.Init();

        var resources = new ResourceManager();
        await resources.LoadAsync();
    }
}
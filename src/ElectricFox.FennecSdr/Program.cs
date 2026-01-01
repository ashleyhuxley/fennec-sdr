using ElectricFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.Display;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.App.Screens;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Graphics;

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
            Console.WriteLine($"Touch Event: {te.Type} at {te.Point.X},{te.Point.Y}");
        };

        touch.Start();
        lcd.Init();

        var resources = new ResourceManager();
        await resources.LoadAsync();

        var screenManager = new ScreenManager(new AppHost(
            new GraphicsRenderer(new LcdScanlineTarget(lcd, 320, 240)),
            touch,
            new SixLabors.ImageSharp.Size(320, 240)
        ));

        screenManager.NavigateTo(new SplashScreen(resources));
        await Task.Delay(2000);

        var menuSelection = await screenManager.ShowAsync(new MainMenuScreen(resources));

        var channel = await screenManager.ShowAsync(new PmrChannelSelectScreen(resources));
        var freq = Constants.PmrChannelFrequencies[channel.Value];

        screenManager.NavigateTo(new CtcssScreen(freq, resources));


    }
}
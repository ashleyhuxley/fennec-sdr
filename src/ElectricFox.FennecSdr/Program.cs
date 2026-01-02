using ElectricFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.Display;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.App.Screens;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

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

        var appHost = new AppHost(
            new GraphicsRenderer(new LcdScanlineTarget(lcd, 320, 240)),
            touch,
            new Size(320, 240)
        );

        var screenManager = new ScreenManager(appHost);

        var splashScreen = new SplashScreen(resources);
        splashScreen.Initialize();
        var mainMenuScreen = new MainMenuScreen(resources);
        mainMenuScreen.Initialize();
        var pmrSelectionScreen = new PmrChannelSelectScreen(resources);
        pmrSelectionScreen.Initialize();
        var ctcssScreen = new CtcssScreen(resources);
        ctcssScreen.Initialize();

        _ = Task.Run(appHost.RunAsync);

        Console.WriteLine("Starting Splash Screen Flow");
        screenManager.NavigateTo(splashScreen);
        await Task.Delay(2000);

        while (true)
        {
            Console.WriteLine("Starting Main Menu Flow");

            var menuSelection = await screenManager.ShowAsync(mainMenuScreen);

            var channel = await screenManager.ShowAsync(pmrSelectionScreen);
            var freq = Constants.PmrChannelFrequencies[channel.Value];

            await screenManager.ShowAsync(ctcssScreen);
        }

    }
}
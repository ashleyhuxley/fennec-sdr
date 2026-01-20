using ElectricFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.Display;
using ElectricFox.FennecSdr.App;
using ElectricFox.FennecSdr.RtlSdrLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        
        var target = new LcdScanlineTarget(lcd, 320, 240);

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            //builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        
       
        // Create app with logging
        var app = new SdrApp(target, touch, new Size(320, 240), loggerFactory, new RtlSdrRadioSource());

        await Task.Run(() => app.StartAsync());
    }
}
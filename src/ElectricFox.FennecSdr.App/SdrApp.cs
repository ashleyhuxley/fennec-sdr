using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Display;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using ElectricFox.FennecSdr.App.Screens;
using ElectricFox.FennecSdr.RtlSdrLib;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App;

public class SdrApp
{
    private readonly AppHost _appHost;
    private readonly IResourceProvider _resourceProvider;
    private readonly IRadioSource _radioSource;
    private readonly CancellationTokenSource _cts = new();
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<SdrApp> _logger;

    public SdrApp(
        IScanlineTarget target, 
        ITouchController touchController,
        Size size,
        ILoggerFactory loggerFactory,
        IRadioSource radioSource)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<SdrApp>();
        
        var renderer = new GraphicsRenderer(target, _loggerFactory.CreateLogger<GraphicsRenderer>());
        _resourceProvider = new ResourceManager();
        
        var appHostLogger = loggerFactory.CreateLogger<AppHost>();
        _appHost = new AppHost(renderer, touchController, size, _resourceProvider, appHostLogger);

        // Create radio source once
        _radioSource = radioSource;
        
        _logger.LogInformation("SdrApp created");
    }

    public async Task StartAsync()
    {
        _logger.LogInformation("Loading resources");
        await _resourceProvider.LoadAsync();
        
        var screenManagerLogger = _loggerFactory.CreateLogger<ScreenManager>();
        var screenManager = new ScreenManager(_appHost, screenManagerLogger);
        
        _ = Task.Run(() => _appHost.RunAsync(_cts.Token));
        
        var splashLogger = _loggerFactory.CreateLogger<SplashScreen>();
        await screenManager.ShowAsync(new SplashScreen(splashLogger));

        while (!_cts.IsCancellationRequested)
        {
            var menuChoice = await screenManager.ShowAsync(new MainMenuScreen());
            switch (menuChoice)
            {
                case MainMenuItem.CtcssToneFinder:
                    var pmrChannel = await screenManager.ShowAsync(new PmrChannelSelectScreen());
                    if (pmrChannel.HasValue)
                    {
                        var ctcssScreen = new CtcssScreen(_radioSource)
                        {
                            Frequency = Constants.PmrChannelFrequencies[pmrChannel.Value]
                        };
                        await screenManager.ShowAsync(ctcssScreen);
                    }
                    break;
                    
                case MainMenuItem.Waterfall:
                    await screenManager.ShowAsync(new FrequencySelectScreen(screenManager));
                    
                    var waterfallScreen = new WaterfallScreen(_radioSource);
                    await screenManager.ShowAsync(waterfallScreen);
                    break;
            }
        }
        
        _logger.LogInformation("Application stopping");
    }
    
    public async Task StopAsync()
    {
        _logger.LogInformation("Stop requested");
        _cts.Cancel();
        
        // Clean up radio source
        await _radioSource.StopAsync();
        _radioSource.Dispose();
    }
}
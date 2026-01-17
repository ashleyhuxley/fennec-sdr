using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Display;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using ElectricFox.FennecSdr.App.Screens;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App;

public class SdrApp
{
    private readonly AppHost _appHost;
    
    private readonly IResourceProvider _resourceProvider;

    private readonly CancellationTokenSource _cts = new();

    public SdrApp(
        IScanlineTarget target, 
        ITouchController touchController,
        Size size)
    {
        var renderer = new GraphicsRenderer(target);
        _resourceProvider = new ResourceManager();
        
        _appHost = new AppHost(renderer, touchController, size, _resourceProvider);
    }

    public async Task StartAsync()
    {
        await _resourceProvider.LoadAsync();
        
        _ = Task.Run(() => _appHost.RunAsync(_cts.Token));
        
        var screenManager = new ScreenManager(_appHost);
        await screenManager.ShowAsync(new SplashScreen());

        while (!_cts.IsCancellationRequested)
        {
            var menuChoice = await screenManager.ShowAsync(new MainMenuScreen());
            switch (menuChoice)
            {
                case MainMenuItem.CtcssToneFinder:
                    var pmrChannel = await  screenManager.ShowAsync(new PmrChannelSelectScreen());
                    if (pmrChannel.HasValue)
                    {
                        var ctcssScreen = new CtcssScreen
                        {
                            Frequency = Constants.PmrChannelFrequencies[pmrChannel.Value]
                        };
                        await screenManager.ShowAsync(ctcssScreen);
                    }

                    break;
                case MainMenuItem.Waterfall:
                    break;
            }
        }
    }
    
    public void Stop() => _cts.Cancel();
}
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using ElectricFox.FennecSdr.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.DesktopTestAvalonia.Views;

public partial class MainWindow : Window, ITouchController
{
    private readonly SdrApp _app;
    private readonly IServiceProvider _serviceProvider;
    
    public MainWindow()
    {
        InitializeComponent();
        
        // Configure logging and dependency injection
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        _serviceProvider = services.BuildServiceProvider();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        
        // Set up rendering target and tie it to the image
        var target = new AvaloniaScanlineTarget(320, 240, Display);
        Display.Source = target.Bitmap;
        
        // Create app with logging
        _app = new SdrApp(target, this, new Size(320, 240), loggerFactory, new FakeRadioSource());

        Display.PointerPressed += (_, args) =>
        {
            var pos = args.GetPosition(Display);
            var point = new TouchPoint(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y));
            TouchEventReceived?.Invoke(new TouchEvent(point));
        };
    }

    public event Action<TouchEvent>? TouchEventReceived;
    
    public void Start()
    {
        
    }

    private async void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        _ = Task.Run(() => _app.StartAsync());
    }
}
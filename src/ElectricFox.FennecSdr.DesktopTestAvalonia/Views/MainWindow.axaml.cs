using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using ElectricFox.FennecSdr.App;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.DesktopTestAvalonia.Views;

public partial class MainWindow : Window, ITouchController
{
    private readonly SdrApp _app;
    
    public MainWindow()
    {
        InitializeComponent();
        
        // Set up rendering target and tie it to the image
        var target = new AvaloniaScanlineTarget(320, 240, Display);
        Display.Source = target.Bitmap;
        
        // Create app
        _app = new SdrApp(target, this, new Size(320, 240));

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
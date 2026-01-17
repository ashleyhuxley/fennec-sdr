using System.Diagnostics;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class AppHost
{
    private readonly GraphicsRenderer _renderer;
    private readonly IResourceProvider _resources;
    private Screen? _current;
    public Size ScreenSize { get; private set; }

    public AppHost(
        GraphicsRenderer renderer, 
        ITouchController touch, 
        Size screenSize,
        IResourceProvider resources)
    {
        _renderer = renderer;
        ScreenSize = screenSize;
        _resources = resources;

        touch.TouchEventReceived += OnTouch;
    }

    public async Task RunAsync(CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        var last = sw.Elapsed;
        
        while (!token.IsCancellationRequested)
        {
            var now = sw.Elapsed;
            Update(now - last);
            last = now;
            
            Render();

            await Task.Delay(16, token); // ~60 FPS max
        }
    }

    public void SetScreen(Screen? screen)
    {
        _current?.OnExit();
        _current = screen;

        if (_current is null)
        {
            return;
        }
        
        _current.OnEnter();
        _renderer.Clear(_current.BackgroundColor);
        _renderer.Flush();
    }

    private void Update(TimeSpan delta)
    {
        _current?.Update(delta);
    }

    private void Render()
    {
        if (_current is null)
        {
            return;
        }
        
        _current?.Render(_renderer, _resources);
        _renderer.Flush();
    }

    private void OnTouch(TouchEvent e)
    {
        _current?.OnTouch(e);
    }
}

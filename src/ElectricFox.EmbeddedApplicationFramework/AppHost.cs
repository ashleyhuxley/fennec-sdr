using System.Diagnostics;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class AppHost
{
    private readonly GraphicsRenderer _renderer;
    private readonly IResourceProvider _resources;
    private readonly ILogger<AppHost> _logger;
    private Screen? _current;
    public Size ScreenSize { get; private set; }

    public AppHost(
        GraphicsRenderer renderer, 
        ITouchController touch, 
        Size screenSize,
        IResourceProvider resources,
        ILogger<AppHost> logger)
    {
        _renderer = renderer;
        ScreenSize = screenSize;
        _resources = resources;
        _logger = logger;

        touch.TouchEventReceived += OnTouch;
        
        _logger.LogInformation("AppHost initialized with screen size {Width}x{Height}", screenSize.Width, screenSize.Height);
    }

    public async Task RunAsync(CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        var last = sw.Elapsed;
        
        _logger.LogInformation("Starting render loop");
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                var now = sw.Elapsed;
                Update(now - last);
                last = now;
                
                Render();

                await Task.Delay(16, token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Render loop cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in render loop");
            }
        }
        
        _logger.LogInformation("Render loop stopped");
    }

    public void SetScreen(Screen? screen)
    {
        var previousScreen = _current?.GetType().Name;
        var newScreen = screen?.GetType().Name ?? "null";
        
        _logger.LogDebug("Changing screen from {PreviousScreen} to {NewScreen}", previousScreen, newScreen);
        
        _current?.OnExit();
        _current = screen;

        if (_current is null)
        {
            _logger.LogWarning("Current screen set to null");
            return;
        }
        
        _current.OnEnter();
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
        _logger.LogTrace("Touch event at ({X}, {Y})", e.Point.X, e.Point.Y);
        _current?.OnTouch(e);
    }
}

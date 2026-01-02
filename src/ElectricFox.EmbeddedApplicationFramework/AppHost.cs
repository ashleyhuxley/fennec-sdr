using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using ElectricFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class AppHost
{
    private readonly GraphicsRenderer _renderer;
    private Screen? _current;
    public Size ScreenSize { get; private set; }

    public AppHost(GraphicsRenderer renderer, ITouchController touch, Size screenSize)
    {
        _renderer = renderer;
        ScreenSize = screenSize;

        touch.TouchEventReceived += OnTouch;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Render();

            await Task.Delay(16); // ~60 FPS max
        }
    }

    public void SetScreen(Screen? screen)
    {
        _current?.OnExit();
        _current = screen;
        _current?.OnEnter();
    }

    public void Update(TimeSpan delta)
    {
        _current?.Update(delta);
    }

    public void Render()
    {
        _current?.Render(_renderer);
        _renderer.Flush();
    }

    private void OnTouch(TouchEvent e)
    {
        _current?.OnTouch(e);
    }
}

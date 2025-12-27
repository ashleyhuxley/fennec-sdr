using ElectrcFox.EmbeddedApplicationFramework.Graphics;
using ElectrcFox.FennecSdr.Touch;
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

    public void SetScreen(Screen screen)
    {
        _current?.OnExit();
        _current = screen;
        _current.OnEnter();

        Render();
    }

    public void Update(TimeSpan delta)
    {
        _current?.Update(delta);
    }

    public void Render()
    {
        _renderer.Clear(Color.Black);
        _current?.Render(_renderer);
        _renderer.Flush();
    }

    private void OnTouch(TouchEvent e)
    {
        _current?.OnTouch(e);
    }
}

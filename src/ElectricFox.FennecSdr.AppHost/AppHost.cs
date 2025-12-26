using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App;

public sealed class AppHost
{
    private readonly GraphicsRenderer _renderer;
    private readonly ITouchController _touch;
    private Screen _current;

    public AppHost(GraphicsRenderer renderer, ITouchController touch)
    {
        _renderer = renderer;
        _touch = touch;

        _touch.TouchEventReceived += OnTouch;
    }

    public void SetScreen(Screen screen)
    {
        _current?.OnExit();
        _current = screen;
        _current.OnEnter();
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

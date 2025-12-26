using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App;

public sealed class AppHost
{
    private readonly GraphicsRenderer _renderer;
    private readonly ITouchController _touch;
    private Screen? _current;
    internal ResourceManager ResourceManager { get; } = new ResourceManager();
    internal Size ScreenSize { get; private set; }

    public AppHost(GraphicsRenderer renderer, ITouchController touch, Size screenSize)
    {
        _renderer = renderer;
        _touch = touch;
        ScreenSize = screenSize;

        _touch.TouchEventReceived += OnTouch;
    }

    public async Task Start()
    {
        await ResourceManager.LoadAsync();
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

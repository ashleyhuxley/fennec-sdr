using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.FennecSdr.App.Ui;

namespace ElectricFox.FennecSdr.App;

public abstract class Screen
{
    protected AppHost App { get; private set; } = null!;

    internal void Attach(AppHost app) => App = app;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual void Update(TimeSpan delta) { }
    public abstract void Render(GraphicsRenderer renderer);
    public virtual void OnTouch(TouchEvent e) { }

    protected UiContainer Controls = new();
}
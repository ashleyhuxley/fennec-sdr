using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public abstract class Screen : UiContainer
{
    protected AppHost App { get; private set; } = null!;

    public Color BackgroundColor { get; set; } = Color.Black;

    internal void Attach(AppHost app) => App = app;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual void Update(TimeSpan delta) { }

    public override void Render(GraphicsRenderer renderer)
    {
        renderer.Clear(BackgroundColor);
        base.Render(renderer);
    }
}

public abstract class Screen<TResult> : Screen
{
    private readonly TaskCompletionSource<TResult> _tcs = new();

    public Task<TResult> Result => _tcs.Task;

    protected void Complete(TResult result)
    {
        _tcs.TrySetResult(result);
    }

    protected void Cancel()
    {
        _tcs.TrySetCanceled();
    }
}
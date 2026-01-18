using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public abstract class Screen : UiContainer
{
    protected AppHost App { get; private set; } = null!;

    public Color BackgroundColor { get; set; } = Color.Black;

    internal void Attach(AppHost app)
    {
        App = app;
        _size = app.ScreenSize;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual void Update(TimeSpan delta) { }

    private Size _size;

    public override Size Size => _size;

    protected abstract void OnInitialize();

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        renderer.FillRect(0, 0, this.Size.Width, this.Size.Height, BackgroundColor);
        base.OnRender(renderer, resourceProvider);
    }
}

public abstract class Screen<TResult> : Screen
{
    private TaskCompletionSource<TResult> _cts = new();

    public Task<TResult> Result => _cts.Task;

    protected void Complete(TResult result)
    {
        _cts.TrySetResult(result);
    }

    protected void Cancel()
    {
        _cts.TrySetCanceled();
    }

    public void Initialize()
    {
        _cts = new TaskCompletionSource<TResult>();
        OnInitialize();
    }
}
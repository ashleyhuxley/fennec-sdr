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

    protected abstract void OnInitialize();
}
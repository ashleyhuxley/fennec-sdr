using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework;

public abstract class Screen : UiContainer
{
    protected AppHost App { get; private set; } = null!;

    internal void Attach(AppHost app) => App = app;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual void Update(TimeSpan delta) { }

    public abstract void Initialize();
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
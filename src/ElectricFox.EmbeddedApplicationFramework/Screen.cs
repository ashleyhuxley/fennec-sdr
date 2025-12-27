using ElectricFox.EmbeddedApplicationFramework.Ui;

namespace ElectricFox.EmbeddedApplicationFramework;

public abstract class Screen : UiContainer
{
    protected AppHost App { get; private set; } = null!;

    internal void Attach(AppHost app) => App = app;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual void Update(TimeSpan delta) { }
}
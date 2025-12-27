namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class ScreenManager
{
    private readonly AppHost _host;

    public ScreenManager(AppHost host)
    {
        _host = host;
    }

    public void NavigateTo(Screen screen)
    {
        screen.Attach(_host);
        _host.SetScreen(screen);
    }
}
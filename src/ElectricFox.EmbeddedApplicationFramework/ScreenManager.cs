namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class ScreenManager
{
    private readonly AppHost _host;

    private readonly Stack<Screen> _stack = new();

    public Screen? Current => _stack.Any() ? _stack.Peek() : null;

    public void Push(Screen screen)
    {
        screen.Attach(_host);
        _stack.Push(screen);
        _host.SetScreen(screen);
    }

    public void Pop()
    {
        if (_stack.Count == 0)
            return;

        _stack.Pop();

        _host.SetScreen(_stack.Count > 0 ? _stack.Peek() : null);
    }

    public ScreenManager(AppHost host)
    {
        _host = host;
    }

    public async Task<TResult> ShowAsync<TResult>(Screen<TResult> screen)
    {
        Push(screen);

        try
        {
            return await screen.Result;
        }
        finally
        {
            Pop();
        }
    }

    public void NavigateTo(Screen screen)
    {
        screen.Attach(_host);
        _host.SetScreen(screen);
    }
}
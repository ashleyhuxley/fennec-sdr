using Microsoft.Extensions.Logging;

namespace ElectricFox.EmbeddedApplicationFramework;

public sealed class ScreenManager
{
    private readonly AppHost _host;
    private readonly ILogger<ScreenManager> _logger;
    private readonly Stack<Screen> _stack = new();

    public Screen? Current => _stack.Any() ? _stack.Peek() : null;

    public ScreenManager(AppHost host, ILogger<ScreenManager> logger)
    {
        _host = host;
        _logger = logger;
    }

    public void Push(Screen screen)
    {
        _logger.LogDebug("Pushing screen {ScreenType}", screen.GetType().Name);
        
        screen.Attach(_host);
        _stack.Push(screen);
        _host.SetScreen(screen);
    }

    public void Pop()
    {
        if (_stack.Count == 0)
        {
            _logger.LogWarning("Attempted to pop from empty screen stack");
            return;
        }

        var screen = _stack.Pop();
        _logger.LogDebug("Popped screen {ScreenType}, stack depth now {Depth}", screen.GetType().Name, _stack.Count);
        Current?.Invalidate();
        _host.SetScreen(Current);
    }

    public async Task<TResult> ShowAsync<TResult>(Screen<TResult> screen)
    {
        _logger.LogInformation("Showing screen {ScreenType}", screen.GetType().Name);
        
        Push(screen);

        try
        {
            screen.Initialize();
            var result = await screen.Result;
            
            _logger.LogInformation("Screen {ScreenType} completed", screen.GetType().Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in screen {ScreenType}", screen.GetType().Name);
            throw;
        }
        finally
        {
            Pop();
        }
    }
}
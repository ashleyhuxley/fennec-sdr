using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class UiContainer : UiElement
{
    private Rectangle? _dirty;
    private ILogger? _logger;

    private List<UiElement> Children { get; } = [];

    public UiContainer()
    {
        // Subscribe to our own Invalidated event to track dirty rectangles
        Invalidated += OnInvalidated;
    }

    protected void SetLogger(ILogger logger) => _logger = logger;

    protected void AddChild(UiElement child)
    {
        child.Parent = this;
        child.Invalidated += OnInvalidated;
        Children.Add(child);
        
        // Call the base Invalidate() instead of OnInvalidated directly
        // This ensures RequiresRedraw gets set
        Invalidate();
    }
    
    public override Size Size
    {
        get
        {
            var width = 0;
            var height = 0;

            foreach (var child in Children)
            {
                width = Math.Max(width, child.Bounds.Right);
                height = Math.Max(height, child.Bounds.Bottom);
            }

            return new Size(width, height);
        }
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        if (!Visible || _dirty == null)
        {
            return;
        }

        _logger?.LogTrace("UiContainer rendering dirty rect: {DirtyRect}", _dirty);
        
        // Clear dirty rect after we've decided to render
        _dirty = null;

        foreach (var child in Children)
        {
            child.Render(renderer, resourceProvider);
        }
    }

    public override bool OnTouch(TouchEvent e)
    {
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            if (Children[i].Bounds.Contains(new Point(e.Point.X, e.Point.Y)) &&
                Children[i].OnTouch(e))
                return true;
        }

        return false;
    }

    private void OnInvalidated(Rectangle rect)
    {
        _dirty = _dirty == null
            ? rect
            : Rectangle.Union(_dirty.Value, rect);
    }
}
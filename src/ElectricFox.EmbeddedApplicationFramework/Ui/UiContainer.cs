using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class UiContainer : UiElement
{
    private ILogger? _logger;

    internal List<UiElement> Children { get; } = [];
    
    /// <summary>
    /// Padding applied to child controls
    /// </summary>
    public int Padding { get; set; } = 0;
    
    /// <summary>
    /// Offset applied to children based on padding
    /// </summary>
    public override Point ChildOffset => new(Padding, Padding);

    protected void SetLogger(ILogger logger) => _logger = logger;

    public void AddChild(UiElement child)
    {
        child.Parent = this;
        Children.Add(child);
        
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
                var childRight = child.Position.X + child.Size.Width;
                var childBottom = child.Position.Y + child.Size.Height;
                width = Math.Max(width, childRight);
                height = Math.Max(height, childBottom);
            }

            return new Size(width + Padding * 2, height + Padding * 2);
        }
    }

    protected override void OnRendered(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        if (!Visible)
        {
            return;
        }

        _logger?.LogTrace("UiContainer children rendering");
        
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
            {
                return true;
            }
        }

        return false;
    }
}
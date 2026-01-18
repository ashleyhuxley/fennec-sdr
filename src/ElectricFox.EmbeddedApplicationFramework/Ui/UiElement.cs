using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public abstract class UiElement
{
    public bool RequiresRedraw { get; protected set; } = true;
    public object? Tag { get; set; }
    
    /// <summary>
    /// Position relative to the parent control
    /// </summary>
    public Point Position { get; set; }
    
    /// <summary>
    /// Absolute position on screen (parent's absolute position + this position + parent offset)
    /// </summary>
    public Point AbsolutePosition
    {
        get
        {
            var parentPos = Parent?.AbsolutePosition ?? Point.Empty;
            var offset = Parent?.ChildOffset ?? Point.Empty;
            return new Point(
                parentPos.X + offset.X + Position.X,
                parentPos.Y + offset.Y + Position.Y
            );
        }
    }
    
    public abstract Size Size { get; }
    
    public Rectangle Bounds => new(AbsolutePosition, Size);
    
    public bool Visible { get; set; } = true;
    public UiElement? Parent { get; set; }
    
    /// <summary>
    /// Offset applied to all child controls (e.g., for padding)
    /// </summary>
    public virtual Point ChildOffset => Point.Empty;

    public event Action<Rectangle>? Invalidated;

    public void Render(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        if (RequiresRedraw)
        {
            OnRender(renderer, resourceProvider);
            RequiresRedraw = false;
        }

        OnRendered(renderer, resourceProvider);
    }

    protected virtual void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
    }

    protected virtual void OnRendered(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
    }

    public virtual bool OnTouch(TouchEvent e) => false;

    protected void Invalidate()
    {
        RequiresRedraw = true;
    }
}
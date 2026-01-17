using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public abstract class UiElement
{
    public bool RequiresRedraw { get; protected set; } = true;
    public object? Tag { get; set; }
    public Point Position { get; set; }
    public abstract Size Size { get; }
    public Rectangle Bounds => new(Position, Size);
    public bool Visible { get; set; } = true;
    public UiElement? Parent { get; set; }

    public event Action<Rectangle>? Invalidated;

    public void Render(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        if (!RequiresRedraw)
        {
            return;
        }
        
        OnRender(renderer, resourceProvider);
        RequiresRedraw = false;
    }

    protected virtual void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
    }
    
    public virtual bool OnTouch(TouchEvent e) => false;

    protected void Invalidate()
    {
        Invalidated?.Invoke(Bounds);
        Parent?.OnChildInvalidated(Bounds);
    }

    protected virtual void OnChildInvalidated(Rectangle rect)
    {
        Invalidated?.Invoke(rect);
        Parent?.OnChildInvalidated(rect);
    }
}
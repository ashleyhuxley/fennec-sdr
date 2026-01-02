using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class UiContainer : UiElement
{
    private Rectangle? _dirty;

    private List<UiElement> Children { get; } = new();

    protected void AddChild(UiElement child)
    {
        child.Parent = this;
        child.Invalidated += OnInvalidated;
        Children.Add(child);
        OnInvalidated(child.Bounds);
    }

    public override Size Size
    {
        get
        {
            int width = 0;
            int height = 0;

            foreach (var child in Children)
            {
                width = Math.Max(width, child.Bounds.Right);
                height = Math.Max(height, child.Bounds.Bottom);
            }

            return new Size(width, height);
        }
    }

    public override void Render(GraphicsRenderer renderer)
    {
        if (!Visible)
            return;

        if (_dirty == null)
            return;

        Console.WriteLine($"UiContainer Render Dirty Rect: {_dirty}");

        OnRender(renderer);

        foreach (var child in Children)
            child.Render(renderer);
    }

    public virtual void OnRender(GraphicsRenderer renderer) { }

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
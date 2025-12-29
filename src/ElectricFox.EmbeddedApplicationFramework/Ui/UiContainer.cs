using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class UiContainer : UiElement
{
    public List<UiElement> Children { get; } = new();

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
        if (!Visible) return;

        foreach (var child in Children)
            child.Render(renderer);
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
}
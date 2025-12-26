using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ElectricFox.FennecSdr.App.Ui;

public sealed class UiContainer : UiElement
{
    public List<UiElement> Children { get; } = new();

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
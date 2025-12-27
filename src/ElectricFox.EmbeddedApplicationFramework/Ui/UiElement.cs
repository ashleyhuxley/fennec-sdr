using ElectrcFox.EmbeddedApplicationFramework.Graphics;
using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public abstract class UiElement
{
    public object? Tag { get; set; }
    public Point Position { get; set; }
    public abstract Size Size { get; }
    public Rectangle Bounds => new(Position, Size);
    public bool Visible { get; set; } = true;

    public virtual void Render(GraphicsRenderer renderer) { }
    public virtual bool OnTouch(TouchEvent e) => false;
}
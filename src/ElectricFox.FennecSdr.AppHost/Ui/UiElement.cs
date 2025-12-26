using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Ui;

public abstract class UiElement
{
    public Rectangle Bounds { get; set; }
    public bool Visible { get; set; } = true;

    public virtual void Render(GraphicsRenderer renderer) { }
    public virtual bool OnTouch(TouchEvent e) => false;
}
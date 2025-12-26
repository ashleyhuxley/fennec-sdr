using ElectrcFox.FennecSdr;
using ElectrcFox.FennecSdr.Touch;
using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Ui;

public sealed class Button : UiElement
{
    public string Text { get; set; } = "";
    public BdfFont? Font { get; set; }
    public event Action? Clicked;

    public override void Render(GraphicsRenderer renderer)
    {
        renderer.DrawRect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Color.White);
        if (Font is not null)
        {
            var rect = Font.MeasureString(Text);
            var posX = Bounds.X + (Bounds.Width / 2 -  rect.Width / 2);
            var posY = Bounds.Y;  // TODO: Use baseline
            renderer.DrawText(Text, Font, posX, posY, Color.Red);
        }
    }

    public override bool OnTouch(TouchEvent e)
    {
        if (e.Type == TouchEventType.Up)
        {
            Clicked?.Invoke();
            return true;
        }

        return false;
    }
}

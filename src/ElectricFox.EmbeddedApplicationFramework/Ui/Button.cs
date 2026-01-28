using ElectricFox.BdfSharp;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.FennecSdr.Touch;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public sealed class Button : UiElement
{
    public string Text { get; set; }
    public BdfFont Font { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public Color BackgroundColor { get; set; } = Color.LightGray;
    public Color BorderColor { get; set; } = Color.DarkGray;


    public event Action<Button>? Clicked;
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 40;

    public override Size Size => new(Width, Height);


    public Button(string text, BdfFont font)
    {
        Text = text;
        Font = font;
    }

    public override void Render(GraphicsRenderer renderer)
    {
        renderer.FillRect(Position.X, Position.Y, Width, Height, BackgroundColor);
        renderer.DrawRect(Position.X, Position.Y, Width, Height, BorderColor);

        var rect = Font.MeasureString(Text);
        var posX = Position.X + (Width / 2 -  rect.Width / 2);
        var posY = Position.Y + (Height / 2 - rect.Height / 2);
        renderer.DrawText(Text, Font, posX, posY, TextColor);
    }

    public override bool OnTouch(TouchEvent e)
    {
        if (e.Type == TouchEventType.Up)
        {
            Clicked?.Invoke(this);
            return true;
        }

        return false;
    }
}

using ElectricFox.BdfSharp;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Label : UiElement
{
    private string _text = "";

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            Invalidate();
        }
    }

    public Color Color { get; set; }
    public BdfFont Font { get; set; }

    public override Size Size
    {
        get
        {
            var rect = Font.MeasureString(Text);
            return new Size(rect.Width, rect.Height);
        }
    }

    public Label(string text, BdfFont font, int x, int y, Color color)
    {
        Text = text;
        Font = font;
        Position = new Point(x, y);
        Color = color;
    }

    public override void Render(GraphicsRenderer renderer)
    {
        renderer.DrawText(Text, Font, Position.X, Position.Y, Color);
    }
}

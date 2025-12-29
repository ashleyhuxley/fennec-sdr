using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Label : UiElement
{
    public string Text { get; set; }
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

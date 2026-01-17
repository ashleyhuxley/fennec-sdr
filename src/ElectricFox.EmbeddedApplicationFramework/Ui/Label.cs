using ElectricFox.BdfSharp;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Label : UiElement
{
    private Size _size;

    public string Text
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    public Color Color { get; set; }
    public string Font { get; set; }
    
    public override Size Size => _size;

    public Label(string text, string font, int x, int y, Color color)
    {
        Text = text;
        Font = font;
        Position = new Point(x, y);
        Color = color;
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        var bdfFont = resourceProvider.GetFont(Font);
        
        var rect = bdfFont.MeasureString(Text);
        _size = new Size(rect.Width, rect.Height);
        
        renderer.DrawText(Text, bdfFont, Position.X, Position.Y, Color);
    }
}

using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;
using ElectricFox.EmbeddedApplicationFramework.Touch;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public sealed class Button : UiElement
{
    public string Text { get; set; }
    public string Font { get; set; }
    public Color TextColor { get; set; } = Color.Black;
    public Color BackgroundColor { get; set; } = Color.LightGray;
    public Color BorderColor { get; set; } = Color.DarkGray;

    public event Action<Button>? Clicked;
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 40;

    public override Size Size => new(Width, Height);

    public Button(string text, string font)
    {
        Text = text;
        Font = font;
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        var bdfFont = resourceProvider.GetFont(Font);
        var absPos = AbsolutePosition;
        
        renderer.FillRect(absPos.X, absPos.Y, Width, Height, BackgroundColor);
        renderer.DrawRect(absPos.X, absPos.Y, Width, Height, BorderColor);

        var rect = bdfFont.MeasureString(Text);
        var posX = absPos.X + (Width / 2 - rect.Width / 2);
        var posY = absPos.Y + (Height / 2 - rect.Height / 2);
        renderer.DrawText(Text, bdfFont, posX, posY, TextColor);
    }

    public override bool OnTouch(TouchEvent e)
    {
        Clicked?.Invoke(this);
        return true;
    }
}

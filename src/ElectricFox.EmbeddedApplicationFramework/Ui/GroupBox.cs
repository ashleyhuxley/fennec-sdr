using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class GroupBox : UiContainer
{
    public string Title { get; set; }
    public string Font { get; set; }
    public Color BorderColor { get; set; } = Color.Gray;
    public Color TextColor { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = Color.Transparent;
    public Color TextBackgroundColor { get; set; } = Color.Black;
    public int BorderWidth { get; set; } = 1;

    public int Width { get; set; } = 150;
    public int Height { get; set; } = 100;

    public override Size Size
    {
        get => new(Width, Height);
    }

    // Padding for the content area (inside the border)
    public int PaddingTop { get; set; } = 10;
    public int PaddingLeft { get; set; } = 5;

    public override Point ChildOffset => new(PaddingLeft, PaddingTop);

    public GroupBox(string title, string font)
    {
        Title = title;
        Font = font;
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        var absPos = AbsolutePosition;
        var size = Size;

        // Draw background if not transparent
        if (BackgroundColor != Color.Transparent)
        {
            renderer.FillRect(absPos.X, absPos.Y, size.Width, size.Height, BackgroundColor);
        }

        var bdfFont = resourceProvider.GetFont(Font);

        var textSize = bdfFont.MeasureString(Title);
        var offsetY = string.IsNullOrEmpty(Title) ? 0 : (textSize.Height + 4) / 2;

        // Draw border
        if (BorderWidth > 0)
        {
            renderer.DrawRect(
                absPos.X,
                absPos.Y + offsetY,
                size.Width,
                size.Height - offsetY,
                BorderColor,
                BorderWidth);
        }

        // Draw title
        if (!string.IsNullOrEmpty(Title))
        {
            var titleX = Width / 2 - textSize.Width / 2;
            var titleY = absPos.Y;

            renderer.FillRect(
                titleX - 3,
                titleY,
                textSize.Width + 6,
                textSize.Height + 4,
                TextBackgroundColor
            );

            renderer.DrawText(Title, bdfFont, titleX, titleY, TextColor);
        }
    }
}

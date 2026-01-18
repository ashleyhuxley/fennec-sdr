using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui
{
    public class TextBlock : UiElement
    {
        public string Text
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }

                field = value;
                Invalidate();
            }
        }

        public Color Color { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public string Font { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int PaddingTop { get; set; } = 3;
        public int PaddingLeft { get; set; } = 3;

        public override Size Size => new(Width, Height);

        public TextBlock(string text, string font, int x, int y, int w, int h, Color color, Color background)
        {
            Text = text;
            Font = font;
            Width = w;
            Height = h;
            BackgroundColor = background;
            Position = new Point(x, y);
            Color = color;
        }

        protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
        {
            var bdfFont = resourceProvider.GetFont(Font);

            renderer.FillRect(AbsolutePosition.X, AbsolutePosition.Y, Width, Height, BackgroundColor);
            renderer.DrawText(Text, bdfFont, AbsolutePosition.X + PaddingLeft, AbsolutePosition.Y + PaddingTop, Color);
        }
    }
}

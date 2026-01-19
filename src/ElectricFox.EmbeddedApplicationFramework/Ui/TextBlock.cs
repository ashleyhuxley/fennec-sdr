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
        public Color BackgroundColor { get; set; }
        public string Font { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int PaddingTop { get; set; } = 3;
        public int PaddingLeft { get; set; } = 3;
        
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;

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
            
            var textSize = bdfFont.MeasureString(Text);
            
            var x = HorizontalAlignment switch
            {
                HorizontalAlignment.Left => PaddingLeft,
                HorizontalAlignment.Center => Size.Width / 2 - textSize.Width / 2,
                HorizontalAlignment.Right => Size.Width - PaddingLeft - textSize.Width,
                _ => 0
            };

            var y = VerticalAlignment switch
            {
                VerticalAlignment.Top => PaddingTop,
                VerticalAlignment.Center => Size.Height / 2 - textSize.Height / 2,
                VerticalAlignment.Bottom => Size.Height - PaddingTop - textSize.Height,
                _ => 0
            };

            renderer.FillRect(AbsolutePosition.X, AbsolutePosition.Y, Width, Height, BackgroundColor);
            renderer.DrawText(Text, bdfFont, AbsolutePosition.X + x, AbsolutePosition.Y + y, Color);
        }
    }
}

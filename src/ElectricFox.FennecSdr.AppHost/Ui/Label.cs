using ElectrcFox.FennecSdr;
using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Ui
{
    public class Label : UiElement
    {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Color.White;
        public BdfFont Font { get; set; }

        public override Size Size
        {
            get
            {
                var rect = Font.MeasureString(Text);
                return new Size(rect.Width, rect.Height);
            }
        }

        public Label(string text, BdfFont font)
        {
            Text = text;
            Font = font;
        }

        public override void Render(GraphicsRenderer renderer)
        {
            renderer.DrawText(Text, Font, Position.X, Position.Y, Color);
        }
    }
}

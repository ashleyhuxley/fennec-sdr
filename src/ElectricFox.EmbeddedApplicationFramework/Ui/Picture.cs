using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Picture : UiElement
{
    public Image<Rgba32> Image { get; set; }

    public override Size Size => new(Image.Width, Image.Height);

    public Picture(Image<Rgba32> image)
    {
        Image = image;
    }

    public override void Render(GraphicsRenderer renderer)
    {
        renderer.DrawImage(Position.X, Position.Y, Image);
    }
}

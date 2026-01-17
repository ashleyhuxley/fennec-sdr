using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Picture : UiElement
{
    private Size _size;
    
    public string Image { get; set; }

    public override Size Size => _size;

    public Picture(string image)
    {
        Image = image;
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        var image = resourceProvider.GetImage(Image);
        _size = image.Size;
        renderer.DrawImage(Position.X, Position.Y, image);
    }
}

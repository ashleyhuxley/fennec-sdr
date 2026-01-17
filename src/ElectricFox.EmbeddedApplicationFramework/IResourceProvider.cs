using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework;

public interface IResourceProvider
{
    Task LoadAsync();
    BdfFont GetFont(string fontName);
    Image<Rgba32> GetImage(string imageName);
}
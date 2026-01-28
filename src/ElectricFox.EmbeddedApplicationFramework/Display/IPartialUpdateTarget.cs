using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Display;

public interface IPartialUpdateTarget
{
    void BeginRegion(Rectangle region);
    void WriteScanline(int y, ReadOnlySpan<Rgba32> data);
    void EndRegion();
}

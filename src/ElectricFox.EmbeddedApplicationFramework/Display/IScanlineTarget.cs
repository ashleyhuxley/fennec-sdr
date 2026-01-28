using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Display;

public interface IScanlineTarget
{
    int Width { get; }
    int Height { get; }
    
    void BeginFrame();
    void WriteScanline(int y, ReadOnlySpan<Rgba32> data);
    void EndFrame();
}

using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Display;

public interface IPartialUpdateTarget
{
    void BeginRegion(Rectangle region);
    void WriteScanline(int y, ReadOnlySpan<byte> rgb565);
    void EndRegion();
}

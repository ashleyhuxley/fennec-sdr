namespace ElectricFox.EmbeddedApplicationFramework.Display;

public interface IScanlineTarget
{
    int Width { get; }
    int Height { get; }

    void BeginFrame();
    void WriteScanline(int y, ReadOnlySpan<byte> rgb565);
    void EndFrame();
}

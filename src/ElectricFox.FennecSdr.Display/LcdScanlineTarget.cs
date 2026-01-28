using ElectricFox.EmbeddedApplicationFramework.Display;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.FennecSdr.Display;

public sealed class LcdScanlineTarget : IScanlineTarget, IPartialUpdateTarget
{
    private readonly ILcdDevice _lcd;

    public int Width => _lcd.Width;
    public int Height => _lcd.Height;

    public LcdScanlineTarget(ILcdDevice lcd)
    {
        _lcd = lcd;
    }

    public void BeginFrame()
    {
        _lcd.SetAddressWindow(0, 0, Width, Height);
        _lcd.BeginWrite();
    }

    public void WriteScanline(int y, ReadOnlySpan<Rgba32> data)
    {
        // Convert Rgba32 data to LCD's pixel format
        int bytesPerPixel = _lcd.PixelConverter.BytesPerPixel;
        Span<byte> buffer = stackalloc byte[data.Length * bytesPerPixel];

        for (int i = 0; i < data.Length; i++)
        {
            int offset = i * bytesPerPixel;
            _lcd.PixelConverter.ConvertPixel(data[i], buffer.Slice(offset, bytesPerPixel));
        }

        _lcd.WriteScanline(buffer);
    }

    public void EndFrame()
    {
        // No-op for most LCD devices
    }

    public void BeginRegion(Rectangle region)
    {
        _lcd.SetAddressWindow(
            region.X,
            region.Y,
            region.Width,
            region.Height);

        _lcd.BeginWrite();
    }

    public void EndRegion()
    {
        // No-op for most LCD devices
    }
}
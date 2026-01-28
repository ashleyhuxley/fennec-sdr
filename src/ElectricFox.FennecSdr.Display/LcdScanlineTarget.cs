using ElectricFox.EmbeddedApplicationFramework.Display;
using SixLabors.ImageSharp;

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

    public void WriteScanline(int y, ReadOnlySpan<byte> rgb565)
    {
        _lcd.WriteScanline(rgb565);
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
}
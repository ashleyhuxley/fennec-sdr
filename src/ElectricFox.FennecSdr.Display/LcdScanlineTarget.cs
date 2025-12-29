using ElectricFox.EmbeddedApplicationFramework.Display;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.Display
{
    public sealed class LcdScanlineTarget : IScanlineTarget, IPartialUpdateTarget
    {
        private readonly Ili9341 _lcd;

        public int Width { get; }
        public int Height { get; }

        public LcdScanlineTarget(Ili9341 lcd, int width, int height)
        {
            _lcd = lcd;
            Width = width;
            Height = height;
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

        public void EndFrame()
        {
            // Method intentionally left empty. LCD does not need EndFrame.
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
            // Not required for this display.
        }
    }

}

using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Graphics;

public class Rgb565PixelConverter : IPixelConverter
{
    public int BytesPerPixel => 2;

    public void ConvertPixel(Rgba32 pixel, Span<byte> destination)
    {
        // Convert RGBA32 to RGB565
        ushort rgb565 = (ushort)(
            ((pixel.R >> 3) << 11) |  // 5 bits red
            ((pixel.G >> 2) << 5) |   // 6 bits green
            (pixel.B >> 3));          // 5 bits blue

        // Write as big-endian (most significant byte first)
        destination[0] = (byte)(rgb565 >> 8);
        destination[1] = (byte)rgb565;
    }
}
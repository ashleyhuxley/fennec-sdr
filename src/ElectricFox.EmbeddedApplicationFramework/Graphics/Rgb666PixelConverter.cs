using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Graphics;

public class Rgb666PixelConverter : IPixelConverter
{
    public int BytesPerPixel => 3;

    public void ConvertPixel(Rgba32 pixel, Span<byte> destination)
    {
        // Convert RGBA32 to RGB666 (18-bit color in 24-bit space)
        // Each component uses 6 bits (values 0-63), shifted left by 2
        destination[0] = (byte)(pixel.R & 0xFC); // Red: top 6 bits
        destination[1] = (byte)(pixel.G & 0xFC); // Green: top 6 bits
        destination[2] = (byte)(pixel.B & 0xFC); // Blue: top 6 bits
    }
}

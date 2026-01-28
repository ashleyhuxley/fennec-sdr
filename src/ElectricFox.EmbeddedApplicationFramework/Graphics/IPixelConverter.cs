using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.EmbeddedApplicationFramework.Graphics;

public interface IPixelConverter
{
    /// <summary>
    /// Number of bytes required per pixel in the target format
    /// </summary>
    int BytesPerPixel { get; }
    
    /// <summary>
    /// Convert a single RGBA32 pixel to the target format
    /// </summary>
    /// <param name="pixel">Source pixel in RGBA32 format</param>
    /// <param name="destination">Destination buffer (must be at least BytesPerPixel bytes)</param>
    void ConvertPixel(Rgba32 pixel, Span<byte> destination);
}

using ElectricFox.EmbeddedApplicationFramework.Display;
using System.Drawing.Imaging;

namespace ElectricFox.FennecSdr.DesktopTest;

public sealed class BitmapScanlineTarget : IScanlineTarget
{
    public int Width { get; }
    public int Height { get; }

    public Bitmap Bitmap { get; }

    public event Action? FrameCompleted;

    public BitmapScanlineTarget(int width, int height)
    {
        Width = width;
        Height = height;
        Bitmap = new Bitmap(width, height, PixelFormat.Format16bppRgb565);
    }

    public void BeginFrame() { }

    public void WriteScanline(int y, ReadOnlySpan<byte> rgb565Line)
    {
        var rect = new Rectangle(0, y, Width, 1);
        var data = Bitmap.LockBits(
            rect,
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            unsafe
            {
                byte* dst = (byte*)data.Scan0;

                for (int x = 0; x < Width; x++)
                {
                    int i = x * 2;
                    ushort rgb565 = (ushort)((rgb565Line[i] << 8) | rgb565Line[i + 1]);

                    byte r = (byte)((rgb565 >> 11) & 0x1F);
                    byte g = (byte)((rgb565 >> 5) & 0x3F);
                    byte b = (byte)(rgb565 & 0x1F);

                    // Expand to 8 bits
                    r = (byte)((r << 3) | (r >> 2));
                    g = (byte)((g << 2) | (g >> 4));
                    b = (byte)((b << 3) | (b >> 2));

                    int o = x * 4;
                    dst[o + 0] = b;     // B
                    dst[o + 1] = g;     // G
                    dst[o + 2] = r;     // R
                    dst[o + 3] = 255;   // A
                }
            }
        }
        finally
        {
            Bitmap.UnlockBits(data);
        }
    }

    public void EndFrame() 
    { 
        FrameCompleted?.Invoke();
    }
}

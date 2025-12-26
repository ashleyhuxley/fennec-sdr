using ElectricFox.BdfSharp;
using ElectricFox.FennecSdr.Display;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ElectrcFox.FennecSdr;

public class GraphicsRenderer
{
    const int Width = 320;
    const int Height = 240;

    private readonly Image<Rgba32> _image;
    private readonly IScanlineTarget _target;

    public GraphicsRenderer(IScanlineTarget target)
    {
        _target = target;
        _image = new Image<Rgba32>(Width, Height);
    }

    public void Clear(Color color)
    {
        _image.Mutate(ctx => ctx.Clear(color));
    }

    public void DrawText(string text, BdfFont font, int x, int y, Color color)
    {
        var data = font.RenderBitmap(text);

        for (int ax = 0; ax < data.GetLength(0); ax++)
        {
            for (int ay = 0; ay < data.GetLength(1); ay++)
            {
                if (data[ax, ay])
                {
                    _image[ax + x, ay + y] = color;
                }
            }
        }
    }

    public void DrawGlyph(int i, BdfFont font, int x, int y, Color color)
    {
        var data = font.RenderBitmap([i], GlyphLookupOption.UseIndex);

        for (int ax = 0; ax < data.GetLength(0); ax++)
        {
            for (int ay = 0; ay < data.GetLength(1); ay++)
            {
                if (data[ax, ay])
                {
                    _image[ax + x, ay + y] = color;
                }
            }
        }
    }

    public void DrawImage(int x, int y, Image<Rgba32> image)
    {
        _image.Mutate(ctx =>
            ctx.DrawImage(image, new Point(x, y), 1f));
    }

    public void DrawRect(int x, int y, int w, int h, Color color, int thickness = 1)
    {
        _image.Mutate(ctx =>
            ctx.Draw(color, thickness, new Rectangle(x, y, w, h)));
    }

    public void FillRect(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Fill(color, new Rectangle(x, y, w, h)));
    }

    public void FillEllipse(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Fill(color, new EllipsePolygon(x + w / 2f, y + h / 2f, w / 2f, h / 2f)));
    }

    public void DrawEllipse(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Draw(color, 1, new EllipsePolygon(x + w / 2f, y + h / 2f, w / 2f, h / 2f)));
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        _image.Mutate(ctx =>
            ctx.DrawLine(color, 1, [new PointF(x1, y1), new PointF(x2, y2)]));
    }

    public void SetPixel(int x, int y, Color color)
    {
        _image[x, y] = color;
    }

    public void Flush()
    {
        _target.BeginFrame();

        Span<byte> line = stackalloc byte[Width * 2];

        for (int y = 0; y < Height; y++)
        {
            // Correct ImageSharp 3.x API
            Span<Rgba32> row = _image.DangerousGetPixelRowMemory(y).Span;

            for (int x = 0; x < Width; x++)
            {
                Rgba32 p = row[x];

                ushort rgb565 =
                    (ushort)(((p.R & 0xF8) << 8) |
                             ((p.G & 0xFC) << 3) |
                             (p.B >> 3));

                int i = x * 2;
                line[i] = (byte)(rgb565 >> 8);
                line[i + 1] = (byte)(rgb565 & 0xFF);
            }

            _target.WriteScanline(y, line);
        }

        _target.EndFrame();
    }
}
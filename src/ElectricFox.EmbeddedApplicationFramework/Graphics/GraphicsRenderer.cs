using ElectricFox.BdfSharp;
using ElectricFox.EmbeddedApplicationFramework.Display;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ElectricFox.EmbeddedApplicationFramework.Graphics;

public class GraphicsRenderer
{
    private readonly Image<Rgba32> _image;
    private readonly IScanlineTarget _target;
    private readonly ILogger<GraphicsRenderer> _logger;
    private Rectangle? _dirty;


    public GraphicsRenderer(IScanlineTarget target, ILogger<GraphicsRenderer> logger)
    {
        _target = target;
        _logger = logger;
        _image = new Image<Rgba32>(_target.Width, _target.Height);
    }

    public void Clear(Color color)
    {
        _logger.LogDebug("Clearing graphics buffer with color {Color}", color);

        _image.Mutate(ctx => ctx.Clear(color));

        MarkDirty(new Rectangle(0, 0, _target.Width, _target.Height));
    }

    private bool IsInBouds(int x, int y)
    {
        return x >= 0 && x < _image.Width && y >= 0 && y < _image.Height;
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
                    if (IsInBouds(ax + x, ay + y))
                    {
                        _image[ax + x, ay + y] = color;
                    }
                }
            }
        }

        MarkDirty(new Rectangle(x, y, data.GetLength(0), data.GetLength(1)));
    }

    public void DrawGlyph(int i, BdfFont font, int x, int y, Color color)
    {
        var data = font.RenderBitmap([i], GlyphLookupOption.UseIndex);

        for (var ax = 0; ax < data.GetLength(0); ax++)
        {
            for (var ay = 0; ay < data.GetLength(1); ay++)
            {
                if (data[ax, ay])
                {
                    _image[ax + x, ay + y] = color;
                }
            }
        }

        MarkDirty(new Rectangle(x, y, data.GetLength(0), data.GetLength(1)));
    }

    public void DrawImage(int x, int y, Image<Rgba32> image)
    {
        _image.Mutate(ctx =>
            ctx.DrawImage(image, new Point(x, y), 1f));

        MarkDirty(new Rectangle(x, y, image.Width, image.Height));
    }

    public void DrawRect(int x, int y, int w, int h, Color color, int thickness = 1)
    {
        _image.Mutate(ctx =>
            ctx.Draw(color, thickness, new Rectangle(x, y, w, h)));

        MarkDirty(new Rectangle(x, y, w, h));
    }

    public void FillRect(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Fill(color, new Rectangle(x, y, w, h)));

        MarkDirty(new Rectangle(x, y, w, h));
    }

    public void FillEllipse(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Fill(color, new EllipsePolygon(x + w / 2f, y + h / 2f, w / 2f, h / 2f)));

        MarkDirty(new Rectangle(x, y, w, h));
    }

    public void DrawEllipse(int x, int y, int w, int h, Color color)
    {
        _image.Mutate(ctx =>
            ctx.Draw(color, 1, new EllipsePolygon(x + w / 2f, y + h / 2f, w / 2f, h / 2f)));

        MarkDirty(new Rectangle(x, y, w, h));
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        _image.Mutate(ctx =>
            ctx.DrawLine(color, 1, new PointF(x1, y1), new PointF(x2, y2)));

        MarkDirty(new Rectangle(
            (int)Math.Min(x1, x2),
            (int)Math.Min(y1, y2),
            (int)Math.Abs(x2 - x1) + 1,
            (int)Math.Abs(y2 - y1) + 1));
    }

    public void SetPixel(int x, int y, Color color)
    {
        _image[x, y] = color;
        MarkDirty(new Rectangle(x, y, 1, 1));
    }

    public void Flush()
    {
        if (_dirty == null)
        {
            return;
        }

        _logger.LogDebug("Flushing graphics buffer to target, dirty region: {Region}", _dirty.Value);

        if (_target is IPartialUpdateTarget partial)
        {
            FlushPartial(partial, _dirty.Value);
        }
        else
        {
            FlushFull();
        }

        _dirty = null;
    }

    private void FlushPartial(IPartialUpdateTarget target, Rectangle r)
    {
        target.BeginRegion(r);

        Span<byte> line = stackalloc byte[r.Width * 2];

        for (int y = r.Top; y < r.Bottom; y++)
        {
            var row = _image.DangerousGetPixelRowMemory(y).Span;

            for (int x = r.Left; x < r.Right; x++)
            {
                var p = row[x];
                ushort rgb565 = ToRgb565(p);

                int i = (x - r.Left) * 2;
                line[i] = (byte)(rgb565 >> 8);
                line[i + 1] = (byte)rgb565;
            }

            target.WriteScanline(y, line);
        }

        target.EndRegion();
    }


    private void FlushFull()
    {
        _target.BeginFrame();

        Span<byte> line = stackalloc byte[_target.Width * 2];

        for (int y = 0; y < _target.Height; y++)
        {
            // Correct ImageSharp 3.x API
            Span<Rgba32> row = _image.DangerousGetPixelRowMemory(y).Span;

            for (int x = 0; x < _target.Width; x++)
            {
                Rgba32 p = row[x];

                ushort rgb565 = ToRgb565(p);

                int i = x * 2;
                line[i] = (byte)(rgb565 >> 8);
                line[i + 1] = (byte)(rgb565 & 0xFF);
            }

            _target.WriteScanline(y, line);
        }

        _target.EndFrame();
    }

    private static ushort ToRgb565(Rgba32 p)
    {
        return (ushort)(((p.R & 0xF8) << 8) |
                        ((p.G & 0xFC) << 3) |
                        (p.B >> 3));
    }

    private void MarkDirty(Rectangle r)
    {
        _dirty = _dirty == null
            ? r
            : Rectangle.Union(_dirty.Value, r);
    }
}
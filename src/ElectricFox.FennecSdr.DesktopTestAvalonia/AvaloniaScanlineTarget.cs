using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ElectricFox.EmbeddedApplicationFramework.Display;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.FennecSdr.DesktopTestAvalonia;

public sealed class AvaloniaScanlineTarget : IScanlineTarget
{
    private readonly WriteableBitmap _bitmap;
    private IDisposable? _lockHandle;

    public int Width { get; }
    public int Height { get; }

    private readonly Image _image;

    public AvaloniaScanlineTarget(int width, int height, Image image)
    {
        Width = width;
        Height = height;
        _image = image;

        _bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);
    }

    public WriteableBitmap Bitmap => _bitmap;

    public void BeginFrame()
    {
        _lockHandle = _bitmap.Lock();
    }

    public unsafe void WriteScanline(int y, ReadOnlySpan<Rgba32> data)
    {
        if (_lockHandle == null)
        {
            throw new InvalidOperationException("BeginFrame must be called before WriteScanline");
        }

        var fb = _lockHandle as ILockedFramebuffer
            ?? throw new InvalidOperationException("Invalid framebuffer lock");

        byte* dst = (byte*)fb.Address + y * fb.RowBytes;

        for (int x = 0; x < Width && x < data.Length; x++)
        {
            var pixel = data[x];

            int offset = x * 4;
            dst[offset + 0] = pixel.B; // Blue
            dst[offset + 1] = pixel.G; // Green
            dst[offset + 2] = pixel.R; // Red
            dst[offset + 3] = 255;//pixel.A; // Alpha (or 255 for opaque)
        }
    }

    private bool _invalidatePending;

    public void EndFrame()
    {
        _lockHandle?.Dispose();
        _lockHandle = null;

        if (_invalidatePending)
            return;

        _invalidatePending = true;

        Dispatcher.UIThread.Post(() =>
        {
            _invalidatePending = false;
            _image.InvalidateVisual();
        });
    }
}

using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ElectricFox.EmbeddedApplicationFramework.Display;

namespace ElectricFox.FennecSdr.DesktopTestAvalonia;

public sealed class AvaloniaScanlineTarget : IScanlineTarget
{
    private readonly WriteableBitmap _bitmap;
    private IDisposable? _lockHandle;

    public int Width { get; }
    public int Height { get; }

    private Image _image;

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

    public unsafe void WriteScanline(int y, ReadOnlySpan<byte> rgb565)
    {
        using var fb = _bitmap.Lock();

        byte* dst = (byte*)fb.Address + y * fb.RowBytes;

        for (int x = 0; x < Width; x++)
        {
            int i = x * 2;
            ushort rgb = (ushort)((rgb565[i] << 8) | rgb565[i + 1]);

            byte r = (byte)((rgb >> 8) & 0xF8);
            byte g = (byte)((rgb >> 3) & 0xFC);
            byte b = (byte)((rgb << 3) & 0xF8);

            int o = x * 4;
            dst[o + 0] = b;
            dst[o + 1] = g;
            dst[o + 2] = r;
            dst[o + 3] = 255;
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

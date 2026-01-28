namespace ElectricFox.FennecSdr.Display
{
    public interface ILcdDevice
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// The pixel converter required by this LCD device
        /// </summary>
        IPixelConverter PixelConverter { get; }

        void SetAddressWindow(int x, int y, int w, int h);
        void BeginWrite();
        void WriteScanline(ReadOnlySpan<byte> data);
    }
}

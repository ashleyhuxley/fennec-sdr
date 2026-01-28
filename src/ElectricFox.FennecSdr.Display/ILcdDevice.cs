namespace ElectricFox.FennecSdr.Display
{
    public interface ILcdDevice
    {
        int Width { get; }
        int Height { get; }
        void SetAddressWindow(int x, int y, int width, int height);
        void BeginWrite();
        void WriteScanline(ReadOnlySpan<byte> data);
    }
}

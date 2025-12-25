namespace ElectrcFox.FennecSdr.Touch
{
    public record TouchCalibration(int MinX, int MaxX, int MinY, int MaxY, bool SwapXY, bool InvertX, bool InvertY)
    {
    }
}

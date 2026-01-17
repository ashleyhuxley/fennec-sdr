namespace ElectricFox.EmbeddedApplicationFramework.Touch;

public readonly struct TouchPoint(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
}

public readonly struct TouchEvent(TouchPoint point)
{
    public TouchPoint Point { get; } = point;
}

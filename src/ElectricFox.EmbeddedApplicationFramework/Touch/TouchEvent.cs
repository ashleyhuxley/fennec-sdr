namespace ElectricFox.EmbeddedApplicationFramework.Touch;

public readonly struct TouchPoint
{
    public int X { get; }
    public int Y { get; }

    public TouchPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public readonly struct TouchEvent
{
    public TouchPoint Point { get; }

    public TouchEvent(TouchPoint point)
    {
        Point = point;
    }
}

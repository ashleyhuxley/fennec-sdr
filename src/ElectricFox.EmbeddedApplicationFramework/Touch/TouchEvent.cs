namespace ElectrcFox.FennecSdr.Touch;

public enum TouchEventType
{
    Down,
    Move,
    Up
}

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
    public TouchEventType Type { get; }
    public TouchPoint Point { get; }

    public TouchEvent(TouchEventType type, TouchPoint point)
    {
        Type = type;
        Point = point;
    }
}

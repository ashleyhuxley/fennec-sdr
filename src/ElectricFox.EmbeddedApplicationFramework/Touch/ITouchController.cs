namespace ElectrcFox.FennecSdr.Touch
{
    public interface ITouchController
    {
        event Action<TouchEvent> TouchEventReceived;
        void Start();
    }
}

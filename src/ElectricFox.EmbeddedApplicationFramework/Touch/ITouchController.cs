namespace ElectricFox.EmbeddedApplicationFramework.Touch;

public interface ITouchController
{
    event Action<TouchEvent> TouchEventReceived;
    void Start();
}
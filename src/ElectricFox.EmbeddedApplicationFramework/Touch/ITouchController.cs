using ElectricFox.EmbeddedApplicationFramework.Touch;

namespace ElectricFox.FennecSdr.Touch
{
    public interface ITouchController
    {
        event Action<TouchEvent> TouchEventReceived;
        void Start();
    }
}

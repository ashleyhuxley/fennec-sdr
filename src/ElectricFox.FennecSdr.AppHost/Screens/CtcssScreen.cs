using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class CtcssScreen : Screen
    {
        private readonly ResourceManager _resourceManager;

        private readonly double _frequency;

        public CtcssScreen(double frequency, ResourceManager resourceManager)
        {
            _frequency = frequency;
            _resourceManager = resourceManager;
        }

        public override void OnEnter()
        {
            Children.Add(new Label("CTCSS Tone Finder", _resourceManager.Profont17, 10, 10, Color.White));
            Children.Add(new Label($"{_frequency:0.###} MHz", _resourceManager.Tamzen8x15b, 10, 30, Color.Red));
        }
    }
}

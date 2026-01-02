using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class CtcssScreen : Screen
    {
        private readonly ResourceManager _resourceManager;

        private double _frequency;

        private Label frequencyLabel;

        public double Frequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                frequencyLabel.Text = $"{_frequency:0.00} MHz";
            }
        }

        public CtcssScreen(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            frequencyLabel = new Label($"0.00 MHz", _resourceManager.Tamzen8x15b, 10, 30, Color.Red);
        }

        public override void Initialize()
        {
            AddChild(
                new Label("CTCSS Tone Finder", _resourceManager.Profont17, 10, 10, Color.White)
            );

            AddChild(frequencyLabel);
        }

        public override void OnEnter() { }
    }
}

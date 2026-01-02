using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class CtcssScreen : Screen<bool>
    {
        private readonly ResourceManager _resourceManager;

        private double _frequency;

        private Label frequencyLabel;
        private Button closeButton;

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
            closeButton = new Button("Close", _resourceManager.Profont17)
            {
                Position = new Point(220, 200),
                Width = 80,
                Height = 30,
                BackgroundColor = Color.Gray,
                BorderColor = Color.White
            };

            closeButton.Clicked += (e) =>
            {
                Complete(true);
            };
        }

        public override void Initialize()
        {
            AddChild(
                new Label("CTCSS Tone Finder", _resourceManager.Profont17, 10, 10, Color.White)
            );

            AddChild(frequencyLabel);

            AddChild(closeButton);
        }

        public override void OnEnter() { }
    }
}

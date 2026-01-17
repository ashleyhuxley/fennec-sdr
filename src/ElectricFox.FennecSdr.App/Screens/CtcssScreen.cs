using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class CtcssScreen : Screen<bool>
{
    private readonly Label _frequencyLabel;
    private readonly Button _closeButton;

    public double Frequency
    {
        get;
        set
        {
            field = value;
            _frequencyLabel.Text = $"{field:0.00000} MHz";
        }
    }

    public CtcssScreen()
    {
        _frequencyLabel = new Label($"0.00 MHz", ResourceManager.BdfFonts.Tamzen8x15b, 10, 30, Color.Red);
        _closeButton = new Button("Close", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(220, 200),
            Width = 80,
            Height = 30,
            BackgroundColor = Color.Gray,
            BorderColor = Color.White
        };

        _closeButton.Clicked += (_) => { Complete(true); };
    }

    protected override void OnInitialize()
    {
        AddChild(
            new Label("CTCSS Tone Finder", ResourceManager.BdfFonts.Profont17, 10, 10, Color.White)
        );

        AddChild(_frequencyLabel);

        AddChild(_closeButton);
    }

    public override void OnEnter()
    {
    }
}
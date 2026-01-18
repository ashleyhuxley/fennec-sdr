using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using ElectricFox.FennecSdr.RtlSdrLib;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class CtcssScreen : Screen<bool>
{
    private readonly Label _frequencyLabel;
    private readonly Label _toneLabel;
    private readonly Button _closeButton;
    private readonly IRadioSource _radioSource;

    public double Frequency
    {
        get;
        set
        {
            field = value;
            _frequencyLabel.Text = $"{field:0.00000} MHz";
            _radioSource.Frequency = (uint)(field * 1_000_000); // Update radio
        }
    }

    public CtcssScreen(IRadioSource radioSource)
    {
        _radioSource = radioSource;
        
        _frequencyLabel = new Label($"0.00 MHz", ResourceManager.BdfFonts.Tamzen8x15b, 10, 30, Color.Red);
        _toneLabel = new Label("Tone: None", ResourceManager.BdfFonts.Tamzen8x15b, 10, 80, Color.Red);
        
        _closeButton = new Button("Close", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(200, 140),
            Width = 80,
            Height = 30,
            BackgroundColor = Color.Gray,
            BorderColor = Color.White
        };

        _closeButton.Clicked += (_) => { Complete(true); };
    }

    protected override void OnInitialize()
    {
        var groupBox = new GroupBox("CTCSS Tone Finder", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(10, 10),
            Width = 300,
            Height = 220,
            BorderColor = Color.White,
            TextColor = Color.White,
            BackgroundColor = Color.Black,
            TextBackgroundColor = Color.Black,
            BorderWidth = 2,
            Padding = 5
        };
        
        AddChild(groupBox);

        groupBox.AddChild(_frequencyLabel);
        groupBox.AddChild(_toneLabel);
        groupBox.AddChild(_closeButton);
        
        // Subscribe to samples
        _radioSource.SamplesAvailable += OnSamplesAvailable;
    }

    public override async void OnEnter()
    {
        // Start the radio when screen becomes active
        await _radioSource.StartAsync(default);
    }

    public override async void OnExit()
    {
        // Stop the radio when leaving screen (but don't dispose!)
        await _radioSource.StopAsync();
        
        // Unsubscribe from samples
        _radioSource.SamplesAvailable -= OnSamplesAvailable;
    }
    
    private void OnSamplesAvailable(short[] samples)
    {
        var tone = Ctcss.DetectCTCSS(samples);
        if (tone.HasValue)
        {
            _toneLabel.Text = $"Tone: {tone.Value:0.00} Hz";
            _toneLabel.Color = Color.Lime;
        }
        else
        {
            _toneLabel.Text = "Tone: None";
            _toneLabel.Color = Color.Red;
        }
    }
}
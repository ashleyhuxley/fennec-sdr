using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using ElectricFox.FennecSdr.RtlSdrLib;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class CtcssScreen : Screen<bool>
{
    private readonly CtcssToneFinder _toneFinder;
    
    private readonly Label _frequencyLabel;
    private readonly Label _toneLabel;
    private readonly Label _lastToneLabel;

    private readonly TextBlock _frequencyText;
    private readonly TextBlock _toneText;
    private readonly TextBlock _lastToneText;

    private readonly Button _closeButton;
    private readonly IRadioSource _radioSource;

    private readonly BarChart _chart;

    private double? _lastTone;

    public double Frequency
    {
        get;
        set
        {
            field = value;
            _frequencyText.Text = $"{field:0.00000} MHz";
            _radioSource.Frequency = (uint)(field * 1_000_000); // Update radio
        }
    }

    public CtcssScreen(IRadioSource radioSource)
    {
        _radioSource = radioSource;
        _toneFinder = new CtcssToneFinder();
        _toneFinder.ToneDetected += ToneFinderOnToneDetected;
        _toneFinder.ToneLost += ToneFinderOnToneLost;
        _toneFinder.HistogramAvailable += ToneFinderOnHistogramAvailable;
        
        _frequencyLabel = new Label($"Frequency:", ResourceManager.BdfFonts.Tamzen8x15b, 0, 2, Color.White);
        _toneLabel = new Label("Tone:", ResourceManager.BdfFonts.Tamzen8x15b, 0, 32, Color.White);
        _lastToneLabel = new Label("Last Tone:", ResourceManager.BdfFonts.Tamzen8x15b, 0, 62, Color.White);

        _frequencyText = new TextBlock("0.00 MHz", ResourceManager.BdfFonts.Profont17, 90, 0, 160, 20, Color.Red, Color.LightGray);
        _toneText = new TextBlock("(None)", ResourceManager.BdfFonts.Profont17, 90, 30, 160, 20, Color.Red, Color.LightGray);
        _lastToneText = new TextBlock("(None)", ResourceManager.BdfFonts.Profont17, 90, 60, 160, 20, Color.Black, Color.LightGray);

        _frequencyText.PaddingLeft = 5;
        _toneText.PaddingLeft = 5;
        _lastToneText.PaddingLeft = 5;

        _closeButton = new Button("Close", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(180, 140),
            Width = 100,
            Height = 40,
            BackgroundColor = Color.Gray,
            BorderColor = Color.White
        };

        _closeButton.Clicked += (_) => { Complete(true); };
        
        _chart = new BarChart(300, 100)
        {
            BarWidth = 3,
            Width = Constants.CtcssTones.Length * 3,
            Height = 90,
            Position = new Point(0, 90),
            BackgroundColor = Color.DarkGray,
            BarColor = Color.Blue,
            SecondaryBarColor = Color.DarkBlue
        };
    }

    private void ToneFinderOnHistogramAvailable((double frequency, double power)[] data)
    {
        _chart.Data = data.Select(d => d.power).ToArray();
        _chart.Invalidate();
    }

    private void ToneFinderOnToneLost()
    {
        _lastToneText.Text = $"{_lastTone:0.00} Hz";
        
        _toneText.Text = "Tone: None";
        _toneText.Color = Color.DarkRed;
    }

    private void ToneFinderOnToneDetected(double tone)
    {
        if (_lastTone is not null)
        {
            _lastToneText.Text = $"{_lastTone:0.00} Hz";
        }
        
        _toneText.Text = $"{tone:0.00} Hz";
        _toneText.Color = Color.DarkGreen;

        _lastTone = tone;
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
            PaddingLeft = 10,
            PaddingTop = 30,
        };
        
        AddChild(groupBox);

        groupBox.AddChild(_frequencyLabel);
        groupBox.AddChild(_toneLabel);
        groupBox.AddChild(_frequencyText);
        groupBox.AddChild(_toneText);
        groupBox.AddChild(_lastToneLabel);
        groupBox.AddChild(_lastToneText);
        groupBox.AddChild(_closeButton);
        groupBox.AddChild(_chart);
        
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
        _toneFinder.AddSamples(samples);
    }
}
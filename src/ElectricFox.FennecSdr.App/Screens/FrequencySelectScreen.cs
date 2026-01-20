using System.Globalization;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class FrequencySelectScreen : Screen<double?>
{
    private readonly ScreenManager _screenManager;
    
    private readonly Label _titleLabel;
    private readonly TextBlock[] _digitTextBlocks = new TextBlock[8];
    private readonly Button[] _upButtons = new Button[8];
    private readonly Button[] _downButtons = new Button[8];
    private readonly Button _okButton;
    private readonly Button _pmrButton;
    private readonly Button _backButton;
    private readonly Button _savedButton;
    private readonly Label _dotLabel;
    private readonly GroupBox _digitsGroup; 
    private readonly byte[] _digits = new byte[8];
    
    public FrequencySelectScreen(ScreenManager screenManager)
    {
        _screenManager = screenManager;
        
        _titleLabel = new Label("Enter frequency:", ResourceManager.BdfFonts.Tamzen8x15b, 15, 10, Color.White);
        
        _digitsGroup = new GroupBox("", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(15, 40),
            BorderWidth = 0,
            Padding = 0,
            Width = 300,
            Height = 120
        };

        for (var i = 0; i < 8; i++)
        {
            var adOffset = i < 3 ? 0 : 15;
            var digitTextBlock = new TextBlock("0", ResourceManager.BdfFonts.Tamzen8x15b, (i * 35) + adOffset, 35, 30, 30,
                Color.Black, Color.LightGrey)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            _digitTextBlocks[i] = digitTextBlock;

            var upButton = new Button("^", ResourceManager.BdfFonts.Tamzen8x15b)
            {
                BackgroundColor = Color.LightCoral,
                BorderColor = Color.DarkGray,
                Height = 30,
                Width = 30,
                Position = new Point((i * 35) + adOffset, 0),
                Tag = i
            };
            _upButtons[i] = upButton;

            var downButton = new Button("v", ResourceManager.BdfFonts.Tamzen8x15b)
            {
                BackgroundColor = Color.LightBlue,
                BorderColor = Color.DarkGray,
                Height = 30,
                Width = 30,
                Position = new Point((i * 35) + adOffset, 70),
                Tag = i
            };
            _downButtons[i] = downButton;
            
            upButton.Clicked += UpButtonOnClicked;
            downButton.Clicked += DownButtonOnClicked;
        }
        
        // PMR button
        _pmrButton = new Button("PMR", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            BackgroundColor = Color.LightSkyBlue,
            BorderColor = Color.DarkGray,
            Height = 60,
            Width = 60,
            Position = new Point(15, 160),
        };
            
        _pmrButton.Clicked += PmrButtonClicked;
        
        // Saved button
        _savedButton = new Button("Presets", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            BackgroundColor = Color.LightSkyBlue,
            BorderColor = Color.DarkGray,
            Height = 60,
            Width = 60,
            Position = new Point(90, 160),
        };
            
        _savedButton.Clicked += SavedButtonClicked;
        
        // Back button
        _backButton = new Button("<", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            BackgroundColor = Color.PaleVioletRed,
            BorderColor = Color.DarkGray,
            Height = 60,
            Width = 60,
            Position = new Point(165, 160),
        };
            
        _backButton.Clicked += BackButtonClicked;
        
        // OK Button
        _okButton = new Button("OK", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            BackgroundColor = Color.LightCyan,
            BorderColor = Color.DarkGray,
            Height = 60,
            Width = 60,
            Position = new Point(240, 160),
        };
            
        _okButton.Clicked += OkButtonOnClicked;

        _dotLabel = new Label(".", ResourceManager.BdfFonts.Tamzen8x15b, 105, 45, Color.White);
    }

    private void OkButtonOnClicked(Button obj)
    {
        var frequency = 0.0;
        foreach (var digit in _digits)
        {
            frequency *= 10;
            frequency += digit;
        }
        frequency /= 1e5;
        Complete(frequency);
    }

    private async void PmrButtonClicked(Button button)
    {
        var pmrChannel = await _screenManager.ShowAsync(new PmrChannelSelectScreen());
        if (!pmrChannel.HasValue)
        {
            return;
        }
        
        var frequency = Constants.PmrChannelFrequencies[pmrChannel.Value - 1];

        var digits = ToFixedDigitArray(frequency);

        for (var j = 0; j < 8; j++)
        {
            _digits[j] = (byte)digits[j];
            _digitTextBlocks[j].Text = $"{_digits[j]}";
        }
    }
    
    public static int[] ToFixedDigitArray(double value)
    {
        // Clamp just in case
        if (value < 0) value = 0;
        if (value > 999.99999) value = 999.99999;

        var digits = new int[8];

        // Integer part (0–999)
        int integerPart = (int)value;

        // Fractional part scaled to 5 digits, truncated
        int fractionalPart = (int)((value - integerPart) * 100000);

        // Fill integer digits (positions 0–2)
        digits[2] = integerPart % 10;
        digits[1] = (integerPart / 10) % 10;
        digits[0] = (integerPart / 100) % 10;

        // Fill fractional digits (positions 3–7)
        for (int i = 7; i >= 3; i--)
        {
            digits[i] = fractionalPart % 10;
            fractionalPart /= 10;
        }

        return digits;
    }

    private void BackButtonClicked(Button button)
    {
        Complete(null);
    }

    private void SavedButtonClicked(Button button)
    {
        
    }

    private void DownButtonOnClicked(Button obj)
    {
        var i = (int)(obj.Tag ?? 0);
        _digits[i] -= 1;
        if (_digits[i] == 255)
        {
            _digits[i] = 9;
        }
        _digitTextBlocks[i].Text = $"{_digits[i]}";
    }

    private void UpButtonOnClicked(Button obj)
    {
        var i = (int)(obj.Tag ?? 0);
        _digits[i] += 1;
        if (_digits[i] == 10)
        {
            _digits[i] = 0;
        }
        _digitTextBlocks[i].Text = $"{_digits[i]}";
    }

    protected override void OnInitialize()
    {
        for (var i = 0; i < 8; i++)
        {
            _digitsGroup.AddChild(_digitTextBlocks[i]);
            _digitsGroup.AddChild(_upButtons[i]);
            _digitsGroup.AddChild(_downButtons[i]);
        }
        
        _digitsGroup.AddChild(_dotLabel);
        AddChild(_pmrButton);
        AddChild(_backButton);
        AddChild(_savedButton);
        AddChild(_okButton);
        AddChild(_digitsGroup);
        AddChild(_titleLabel);
    }
}
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class PmrChannelSelectScreen : Screen<int?>
{
    protected override void OnInitialize()
    {
        var label = new Label(
            "Select PMR channel",
            ResourceManager.BdfFonts.Tamzen8x15b,
            12,
            10,
            Color.SkyBlue
        );
        AddChild(label);

        const int gridWidth = 75;
        const int gridHeight = 50;
        const int offsetX = 12;
        const int offsetY = 35;

        var channel = 1;

        for (var y = 0; y < 4; y++)
        {
            for (var x = 0; x < 4; x++)
            {
                var ox = x * gridWidth + offsetX;
                var oy = y * gridHeight + offsetY;

                Color col;
                if (x % 2 == 0)
                {
                    col = y % 2 == 0 ? Color.Coral : Color.CornflowerBlue;
                }
                else
                {
                    col = y % 2 == 0 ? Color.CornflowerBlue : Color.Coral;
                }

                var button = new Button($"{channel}", ResourceManager.BdfFonts.Profont17)
                {
                    Position = new Point(ox, oy),
                    Width = gridWidth - 5,
                    Height = gridHeight - 5,
                    Tag = channel,
                    BackgroundColor = col,
                    BorderColor = Color.White
                };

                button.Clicked += Button_Clicked;

                AddChild(button);

                channel++;
            }
        }
        
        Console.WriteLine("PMR Selection Screen initialized");
    }

    public override void OnEnter()
    {
    }

    private void Button_Clicked(Button obj)
    {
        var channel = obj.Tag as int?;

        Complete(channel);
    }
}
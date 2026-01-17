using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public enum MainMenuItem
{
    CtcssToneFinder,
    Waterfall,
    SMeter,
    Exit
}

public class MainMenuScreen : Screen<MainMenuItem>
{
    protected override void OnInitialize()
    {
        var groupBox = new GroupBox("Main Menu", ResourceManager.BdfFonts.Profont17)
        {
            Position = new Point(10, 10),
            BorderColor = Color.White,
            TextColor = Color.FromRgb(30, 0, 255),
            BorderWidth = 2,
            Padding = 4,
            Width = 300,
            Height = 210
        };

        AddChild(groupBox);

        var ctcssButton = new Button("CTCSS", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            Tag = MainMenuItem.CtcssToneFinder,
            Position = new Point(10, 10),
            Width = 120,
            Height = 60
        };

        var waterfallButton = new Button("Waterfall", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            Tag = MainMenuItem.Waterfall,
            Position = new Point(140, 10),
            Width = 120,
            Height = 60
        };

        var sMeterButton = new Button("S-Meter", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            Tag = MainMenuItem.SMeter,
            Position = new Point(10, 80),
            Width = 120,
            Height = 60
        };

        var exitButton = new Button("Exit", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            Tag = MainMenuItem.Exit,
            Position = new Point(140, 80),
            Width = 120,
            Height = 60
        };

        ctcssButton.Clicked += Button_Clicked;
        waterfallButton.Clicked += Button_Clicked;
        sMeterButton.Clicked += Button_Clicked;
        exitButton.Clicked += Button_Clicked;

        groupBox.AddChild(ctcssButton);
        groupBox.AddChild(waterfallButton);
        groupBox.AddChild(sMeterButton);
        groupBox.AddChild(exitButton);
    }

    public override void OnEnter()
    {
    }

    private void Button_Clicked(Button obj)
    {
        if (obj.Tag is null)
        {
            return;
        }

        Complete((MainMenuItem)obj.Tag);
    }
}
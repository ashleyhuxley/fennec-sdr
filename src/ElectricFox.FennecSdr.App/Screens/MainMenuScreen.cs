using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public enum MainMenuItem
{
    CtcssToneFinder,
    Waterfall
}

public class MainMenuScreen : Screen<MainMenuItem>
{
    protected override void OnInitialize()
    {
        var ctcssButton = new Button("CTCSS", ResourceManager.BdfFonts.Tamzen8x15b)
        {
            Tag = MainMenuItem.CtcssToneFinder,
            Position = new Point(10, 10),
            Width = 120,
            Height = 60
        };

        ctcssButton.Clicked += CtcssButton_Clicked;

        AddChild(ctcssButton);
    }

    public override void OnEnter()
    {
    }

    private void CtcssButton_Clicked(Button obj)
    {
        if (obj.Tag is null)
        {
            return;
        }

        Complete((MainMenuItem)obj.Tag);
    }
}
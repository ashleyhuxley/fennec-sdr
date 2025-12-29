using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public enum MainMenuItem
    {
        CtcssToneFinder,
        Waterfall
    }

    public class MainMenuScreen : Screen<MainMenuItem>
    {
        private readonly ResourceManager _resourceManager;

        public MainMenuScreen(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public override void OnEnter()
        {
            var ctcssButton = new Button("CTCSS", _resourceManager.Tamzen8x15b)
            {
                Tag = MainMenuItem.CtcssToneFinder,
                Position = new Point(10, 10),
                Width = 90,
                Height = 20
            };

            ctcssButton.Clicked += CtcssButton_Clicked;

            Children.Add(ctcssButton);
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
}

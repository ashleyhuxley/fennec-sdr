using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class PmrChannelSelectScreen : Screen<int?>
    {
        private readonly ResourceManager _resourceManager;

        public PmrChannelSelectScreen(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public override void OnEnter()
        {
            var label = new Label(
                "Select PMR channel",
                _resourceManager.Tamzen8x15b,
                12,
                10,
                Color.SkyBlue
            );
            Children.Add(label);

            const int GridWidth = 75;
            const int GridHeight = 50;
            const int OffsetX = 12;
            const int OffsetY = 35;

            int channel = 1;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var ox = x * GridWidth + OffsetX;
                    var oy = y * GridHeight + OffsetY;

                    Color col;
                    if (x % 2 == 0)
                    {
                        col = y % 2 == 0 ? Color.Coral : Color.CornflowerBlue;
                    }
                    else
                    {
                        col = y % 2 == 0 ? Color.CornflowerBlue : Color.Coral;
                    }

                    var button = new Button($"{channel}", _resourceManager.Profont17)
                    {
                        Position = new Point(ox, oy),
                        Width = GridWidth - 5,
                        Height = GridHeight - 5,
                        Tag = channel,
                        BackgroundColor = col,
                        BorderColor = Color.White
                    };

                    button.Clicked += Button_Clicked;

                    Children.Add(button);

                    channel++;
                }
            }
        }

        private void Button_Clicked(Button obj)
        {
            var channel = obj.Tag as int?;

            Complete(channel);
        }
    }
}

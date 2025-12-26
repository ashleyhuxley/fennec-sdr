using ElectrcFox.FennecSdr;
using ElectricFox.FennecSdr.App.Ui;
using SixLabors.ImageSharp;
using System.Threading.Channels;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class MainScreen : Screen
    {
        private readonly ScreenManager _manager;

        public MainScreen(ScreenManager manager)
        {
            _manager = manager;
        }

        public override void Render(GraphicsRenderer renderer)
        {
            const int GridSize = 50;
            const int OffsetX = 10;
            const int OffsetY = 10;

            int channel = 1;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var ox = x * GridSize + OffsetX;
                    var oy = y * GridSize + OffsetY;

                    var col = x % 2 == 0 ? (y % 2 == 0 ? Color.CornflowerBlue : Color.Coral) : (y % 2 != 0 ? Color.CornflowerBlue : Color.Coral);

                    var button = new Button($"{channel}", App.ResourceManager.Profont17)
                    {
                        Position = new Point(ox, oy),
                        Width = 40,
                        Height = 40,
                    };

                    Controls.Children.Add(button);

                    channel++;
                }
            }

            Controls.Render(renderer);
        }
    }
}

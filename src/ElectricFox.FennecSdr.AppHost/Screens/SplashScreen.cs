using ElectrcFox.FennecSdr;
using ElectricFox.FennecSdr.App.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens
{
    public class SplashScreen : Screen
    {
        private static readonly Random _rng = new();

        private readonly ScreenManager _manager;

        public SplashScreen(ScreenManager manager)
        {
            _manager = manager;
            Timer timer = new(Callback, null, TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
        }

        private void Callback(object? state)
        {
            _manager.NavigateTo(new MainScreen(_manager));
        }

        public override void Render(GraphicsRenderer renderer)
        {
            var canvas = new Canvas(this.App.ScreenSize.Width, this.App.ScreenSize.Height);
            canvas.Rendered += Canvas_Rendered;

            Controls.Children.Add(canvas);

            Controls.Children.Add(
                new Picture(App.ResourceManager.Fennec) { Position = new Point(158, 15) }
            );

            Controls.Children.Add(
                new Label("145.400", App.ResourceManager.Profont17)
                {
                    Position = new Point(33, 42),
                    Color = Color.FromRgb(56, 232, 46),
                }
            );

            Controls.Children.Add(
                new Label("MHz", App.ResourceManager.Profont17)
                {
                    Position = new Point(105, 42),
                    Font = App.ResourceManager.Profont17,
                }
            );

            Controls.Children.Add(
                new Label("FENNEC", App.ResourceManager.CalBlk36)
                {
                    Position = new Point(25, 143),
                    Color = Color.FromRgb(252, 111, 0),
                }
            );

            Controls.Children.Add(
                new Label("SDR", App.ResourceManager.CalBlk36)
                {
                    Position = new Point(25, 175),
                    Color = Color.White,
                }
            );

            Controls.Render(renderer);
        }

        private void Canvas_Rendered(GraphicsRenderer renderer)
        {
            renderer.DrawRect(25, 35, 270, 100, Color.DarkGrey, 2);
            renderer.DrawGlyph(6, App.ResourceManager.OpenIconicOther2x!, 115, 186, Color.Yellow);

            for (int y = 70; y < 120; y += 10)
            {
                renderer.DrawLine(33, y, 155, y, y == 70 ? Color.Blue : Color.LightGrey);
            }

            renderer.DrawRect(33, 120, 122, 9, Color.LightGreen);
            renderer.FillRect(35, 122, 90, 6, Color.LightGreen);

            var data = GetMultiModalList(25, 300, 50).ToList();

            for (int i = 0; i < data.Count - 1; i++)
            {
                int x1 = 35 + i * 5;
                int y1 = 115 - data[i];
                renderer.FillRect(x1, y1, 4, data[i], Color.LightBlue);
            }
        }

        private static IEnumerable<int> GetMultiModalList(int count, int samples, int max)
        {
            List<int> list = new();
            for (int i = 0; i < samples; i++)
            {
                list.Add(Convert.ToInt32(RandomSharpPeak(0.0f, count)));
            }

            for (int i = 0; i < count; i++)
            {
                yield return Math.Clamp(list.Count(l => l == i), 1, max);
            }
        }

        public static float RandomSharpPeak(
            float minValue = 0.0f,
            float maxValue = 1.0f,
            float sharpness = 1.4f
        )
        {
            float u,
                v,
                S;

            do
            {
                u = 2.0f * (float)_rng.NextDouble() - 1.0f;
                v = 2.0f * (float)_rng.NextDouble() - 1.0f;
                S = u * u + v * v;
            } while (S >= 1.0f || S == 0f);

            // Standard Normal Distribution
            float std = (float)(u * Math.Sqrt(-2.0f * Math.Log(S) / S));

            // 1. Calculate Mean and Sigma
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;

            // 2. Apply Sharpness
            // We normalize the 'std', apply a power to push values toward 0,
            // then re-scale by sigma and move to mean.
            // High sharpness (e.g., 3.0) creates a needle-like peak.
            float sign = Math.Sign(std);
            float sharpened = sign * (float)Math.Pow(Math.Abs(std), 1.0f / sharpness);

            // Note: To get a SHARP peak (more values at center),
            // we actually want to shrink the 'std' result before scaling.
            // Alternatively, just reduce sigma significantly:
            float tightSigma = sigma / sharpness;

            return Math.Clamp(std * tightSigma + mean, minValue, maxValue);
        }
    }
}

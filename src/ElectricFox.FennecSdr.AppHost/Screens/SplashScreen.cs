using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class SplashScreen : Screen
{
    private static readonly Random _rng = new();

    private readonly ResourceManager _resourceManager;

    public SplashScreen(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public override void OnEnter()
    {

    }

    private void Canvas_Rendered(GraphicsRenderer renderer)
    {
        renderer.DrawRect(25, 35, 270, 100, Color.DarkGrey, 2);
        renderer.DrawGlyph(6, _resourceManager.OpenIconicOther2x!, 115, 186, Color.Yellow);

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
        } while (S >= 1.0f || (S - 0f < 0.0001f));

        float std = (float)(u * Math.Sqrt(-2.0f * Math.Log(S) / S));

        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;

        float tightSigma = sigma / sharpness;

        return Math.Clamp(std * tightSigma + mean, minValue, maxValue);
    }

    public override void Initialize()
    {
        var canvas = new Canvas(320, 240);
        canvas.Rendered += Canvas_Rendered;

        AddChild(canvas);

        AddChild(new Picture(_resourceManager.Fennec) { Position = new Point(158, 15) });

        AddChild(
            new Label(
                "145.400",
                _resourceManager.Profont17,
                33,
                42,
                Color.FromRgb(56, 232, 46)
            )
        );

        AddChild(new Label("MHz", _resourceManager.Profont17, 105, 42, Color.White));

        AddChild(
            new Label(
                "FENNEC",
                _resourceManager.CalBlk36,
                25,
                143,
                Color.FromRgb(252, 111, 0)
            )
        );

        AddChild(new Label("SDR", _resourceManager.CalBlk36, 25, 175, Color.White));
    }
}

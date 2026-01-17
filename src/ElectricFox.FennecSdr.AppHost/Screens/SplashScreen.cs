using System.Diagnostics;
using ElectricFox.EmbeddedApplicationFramework.Graphics;
using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.EmbeddedApplicationFramework.Ui;
using SixLabors.ImageSharp;

namespace ElectricFox.FennecSdr.App.Screens;

public class SplashScreen : Screen<object?>
{
    private static readonly Random Rng = new();

    private readonly Stopwatch _stopwatch = new();

    public override void OnEnter()
    {
        _stopwatch.Restart();
    }

    public override void Update(TimeSpan delta)
    {
        if (_stopwatch.Elapsed > TimeSpan.FromSeconds(3))
        {
            Complete(null);
        }
        
        base.Update(delta);
    }

    private void Canvas_Rendered(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        Console.WriteLine("SplashScreen canvas render");
        
        var glyphFont = resourceProvider.GetFont(ResourceManager.BdfFonts.OpenIconicOther2X);
        
        renderer.DrawRect(25, 35, 270, 100, Color.DarkGrey, 2);
        renderer.DrawGlyph(6, glyphFont, 115, 186, Color.Yellow);

        for (var y = 70; y < 120; y += 10)
        {
            renderer.DrawLine(33, y, 155, y, y == 70 ? Color.Blue : Color.LightGrey);
        }

        renderer.DrawRect(33, 120, 122, 9, Color.LightGreen);
        renderer.FillRect(35, 122, 90, 6, Color.LightGreen);

        var data = GetMultiModalList(25, 300, 50).ToList();

        for (var i = 0; i < data.Count - 1; i++)
        {
            var x1 = 35 + i * 5;
            var y1 = 115 - data[i];
            renderer.FillRect(x1, y1, 4, data[i], Color.LightBlue);
        }
    }

    private static IEnumerable<int> GetMultiModalList(int count, int samples, int max)
    {
        List<int> list = [];
        for (var i = 0; i < samples; i++)
        {
            list.Add(Convert.ToInt32(RandomSharpPeak(0.0f, count)));
        }

        for (var i = 0; i < count; i++)
        {
            yield return Math.Clamp(list.Count(l => l == i), 1, max);
        }
    }

    private static float RandomSharpPeak(
        float minValue = 0.0f,
        float maxValue = 1.0f,
        float sharpness = 1.4f
    )
    {
        float u;
        float s;

        do
        {
            u = 2.0f * (float)Rng.NextDouble() - 1.0f;
            var v = 2.0f * (float)Rng.NextDouble() - 1.0f;
            s = u * u + v * v;
        } while (s >= 1.0f || (s - 0f < 0.0001f));

        var std = (float)(u * Math.Sqrt(-2.0f * Math.Log(s) / s));

        var mean = (minValue + maxValue) / 2.0f;
        var sigma = (maxValue - mean) / 3.0f;

        var tightSigma = sigma / sharpness;

        return Math.Clamp(std * tightSigma + mean, minValue, maxValue);
    }

    protected override void OnInitialize()
    {
        Console.WriteLine("SplashScreen start initialize");
        
        var canvas = new Canvas(320, 240);
        canvas.Rendered += Canvas_Rendered;

        AddChild(canvas);

        AddChild(new Picture(ResourceManager.Images.Fennec) { Position = new Point(158, 15) });

        AddChild(
            new Label(
                "145.400",
                ResourceManager.BdfFonts.Profont17,
                33,
                42,
                Color.FromRgb(56, 232, 46)
            )
        );

        AddChild(new Label("MHz", ResourceManager.BdfFonts.Profont17, 105, 42, Color.White));

        AddChild(
            new Label(
                "FENNEC",
                ResourceManager.BdfFonts.CalBlk36,
                25,
                143,
                Color.FromRgb(252, 111, 0)
            )
        );

        AddChild(new Label("SDR", ResourceManager.BdfFonts.CalBlk36, 25, 175, Color.White));

        RequiresRedraw = true;
        
        Console.WriteLine("SplashScreen end initialize");
    }
}

using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class BarChart : UiElement
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public Color BarColor { get; set; } = Color.White;
    public Color SecondaryBarColor { get; set; } = Color.Gray;
    public Color BackgroundColor { get; set; } = Color.Black;
    public override Size Size => new(Width, Height);
    
    public double[]? Data { get; set; }
    public int BarWidth { get; set; } = 10;
    
    public BarChart(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        renderer.FillRect(AbsolutePosition.X, AbsolutePosition.Y, Width, Height, BackgroundColor);

        if (Data is null)
        {
            return;
        }

        for (var i = 0; i < Data.Length; i++)
        {
            var barHeight = Convert.ToInt32(Data[i] * (Height / Data.Max()));
            var colour = i % 2 == 0 ? SecondaryBarColor : BarColor;
            renderer.FillRect(AbsolutePosition.X + i * BarWidth, AbsolutePosition.Y + Height - barHeight, BarWidth, barHeight, colour);
        }
    }
}
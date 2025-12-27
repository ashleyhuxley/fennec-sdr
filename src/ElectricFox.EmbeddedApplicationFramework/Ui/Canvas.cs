using ElectrcFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Canvas : UiElement
{
    public override Size Size => new(Width, Height);

    public event Action<GraphicsRenderer>? Rendered;

    public int Width { get; }

    public int Height { get; }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override void Render(GraphicsRenderer renderer)
    {
        Rendered?.Invoke(renderer);
    }
}

using ElectricFox.EmbeddedApplicationFramework.Graphics;
using SixLabors.ImageSharp;

namespace ElectricFox.EmbeddedApplicationFramework.Ui;

public class Canvas : UiElement
{
    public override Size Size => new(Width, Height);

    public event Action<GraphicsRenderer, IResourceProvider>? Rendered;

    public int Width { get; }

    public int Height { get; }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
    }

    protected override void OnRender(GraphicsRenderer renderer, IResourceProvider resourceProvider)
    {
        Rendered?.Invoke(renderer, resourceProvider);
    }
}

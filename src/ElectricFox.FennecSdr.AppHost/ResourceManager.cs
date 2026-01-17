using ElectricFox.BdfSharp;
using ElectricFox.EmbeddedApplicationFramework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.FennecSdr.App;

public class ResourceManager : IResourceProvider
{
    public const string AppName = "FennecSDR";
    private const string NotLoadedError = "Resources not loaded";

    public class BdfFonts
    {
        public static string Tamzen8x15b = "ElectricFox.FennecSdr.App.Resources.Tamzen8x15b.bdf";
        public static string CalBlk36 = "ElectricFox.FennecSdr.App.Resources.CalBlk36.bdf";
        public static string Profont17 = "ElectricFox.FennecSdr.App.Resources.profont17.bdf";
        public static string OpenIconicOther2X = "ElectricFox.FennecSdr.App.Resources.open_iconic_other_2x.bdf";
    }

    public class Images
    {
        public static string Fennec = "ElectricFox.FennecSdr.App.Resources.fennec.png";
    }

    private Dictionary<string, BdfFont> _fontCache = new();
    private Dictionary<string, Image<Rgba32>> _imageCache = new();

    public async Task LoadAsync()
    {
        await AddFont(BdfFonts.Tamzen8x15b);
        await AddFont(BdfFonts.CalBlk36);
        await AddFont(BdfFonts.Profont17);
        await AddFont(BdfFonts.OpenIconicOther2X);
        
        await AddImage(Images.Fennec);
    }

    private async Task AddFont(string font)
    {
        var assembly = typeof(ResourceManager).Assembly;

        var bdfFont = await BdfFont.LoadFromEmbeddedResourceAsync(
            font,
            assembly
        );
        
        _fontCache[font] = bdfFont;
    }

    private async Task AddImage(string imageName)
    {
        var assembly = typeof(ResourceManager).Assembly;

        var imageData = await Image.LoadAsync<Rgba32>(
            assembly.GetManifestResourceStream(imageName)!
        );
        
        _imageCache[imageName] = imageData;
    }

    public BdfFont GetFont(string font)
    {
        return _fontCache.TryGetValue(font, out var value) ? value : throw new InvalidOperationException(NotLoadedError);
    }

    public Image<Rgba32> GetImage(string imageName)
    {
        return _imageCache.TryGetValue(imageName, out var value) ? value : throw new InvalidOperationException(NotLoadedError);
    }
}

using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.FennecSdr.App
{
    internal class ResourceManager
    {
        public const string AppName = "FennecSDR";
        public BdfFont? Tamzen8x15b { get; private set; }
        public BdfFont? CalBlk36 { get; private set; }
        public BdfFont? Profont17 { get; private set; }
        public BdfFont? OpenIconicOther2x { get; private set; }
        public Image<Rgba32>? Fennec { get; private set; }

        public ResourceManager() { }

        public async Task LoadAsync()
        {
            var assembly = typeof(AppHost).Assembly;

            Tamzen8x15b = await BdfFont.LoadFromEmbeddedResourceAsync(
                "ElectricFox.FennecSdr.App.Resources.Tamzen8x15b.bdf",
                assembly
            );

            CalBlk36 = await BdfFont.LoadFromEmbeddedResourceAsync(
                "ElectricFox.FennecSdr.App.Resources.CalBlk36.bdf",
                assembly
            );

            Profont17 = await BdfFont.LoadFromEmbeddedResourceAsync(
                "ElectricFox.FennecSdr.App.Resources.profont17.bdf",
                assembly
            );

            OpenIconicOther2x = await BdfFont.LoadFromEmbeddedResourceAsync(
                "ElectricFox.FennecSdr.App.Resources.open_iconic_other_2x.bdf",
                assembly
            );

            Fennec = await Image.LoadAsync<Rgba32>(
                assembly.GetManifestResourceStream("ElectricFox.FennecSdr.App.Resources.fennec.png")!
            );
        }
    }
}

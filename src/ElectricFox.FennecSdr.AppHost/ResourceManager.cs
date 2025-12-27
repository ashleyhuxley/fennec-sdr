using ElectricFox.BdfSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ElectricFox.FennecSdr.App
{
    public class ResourceManager
    {
        public const string AppName = "FennecSDR";
        private const string NotLoadedError = "Resources not loaded";

        private BdfFont? _tamzen8x15;
        public BdfFont Tamzen8x15b
        {
            get => _tamzen8x15 ?? throw new InvalidOperationException(NotLoadedError);
            private set => _tamzen8x15 = value;
        }

        private BdfFont? _calBlk36;
        public BdfFont CalBlk36
        { 
            get => _calBlk36 ?? throw new InvalidOperationException(NotLoadedError); 
            private set => _calBlk36 = value;
        }

        private BdfFont? _profont17;
        public BdfFont Profont17
        {
            get => _profont17 ?? throw new InvalidOperationException(NotLoadedError);
            private set => _profont17 = value;
        }

        private BdfFont? _openIconicOther2x;
        public BdfFont OpenIconicOther2x
        {
            get => _openIconicOther2x ?? throw new InvalidOperationException(NotLoadedError);
            private set => _openIconicOther2x = value;
        }

        private Image<Rgba32>? _fennec;
        public Image<Rgba32> Fennec 
        {
            get => _fennec ?? throw new InvalidOperationException(NotLoadedError); 
            private set => _fennec = value; 
        }

        public ResourceManager() { }

        public async Task LoadAsync()
        {
            var assembly = typeof(ResourceManager).Assembly;

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

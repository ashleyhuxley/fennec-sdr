using ElectricFox.EmbeddedApplicationFramework;
using ElectricFox.FennecSdr.RtlSdrLib;

namespace ElectricFox.FennecSdr.App.Screens;

public class WaterfallScreen : Screen<object>
{
    private readonly IRadioSource _radioSource;

    public WaterfallScreen(IRadioSource radioSource)
    {
        _radioSource = radioSource;
    }

    protected override void OnInitialize()
    {
        
    }
}

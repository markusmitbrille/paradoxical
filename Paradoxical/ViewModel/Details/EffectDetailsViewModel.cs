using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class EffectDetailsViewModel : PageViewModel
{
    public override string PageName => "Effect Details";

    public IEffectService Service { get; }

    public EffectDetailsViewModel(NavigationViewModel navigation, IEffectService service)
        : base(navigation)
    {
        Service = service;
    }
}
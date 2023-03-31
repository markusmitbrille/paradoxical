using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class EffectTableViewModel : PageViewModel
{
    public override string PageName => "Effects";

    public IEffectService Service { get; }

    public EffectTableViewModel(NavigationViewModel navigation, IEffectService service)
        : base(navigation)
    {
        Service = service;
    }
}
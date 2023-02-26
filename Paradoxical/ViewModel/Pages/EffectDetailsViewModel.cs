using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Effect Details";

    public IEffectService Service { get; }

    public EffectDetailsViewModel(NavigationViewModel navigation, IEffectService service)
        : base(navigation)
    {
        Service = service;
    }
}
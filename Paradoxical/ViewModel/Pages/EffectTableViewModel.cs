using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectTableViewModel : PageViewModelBase
{
    public override string PageName => "Effects";

    public IEffectService Service { get; }

    public EffectTableViewModel(IEffectService service)
    {
        Service = service;
    }
}
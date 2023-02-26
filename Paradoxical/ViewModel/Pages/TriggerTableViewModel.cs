using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerTableViewModel : PageViewModelBase
{
    public override string PageName => "Triggers";

    public ITriggerService Service { get; }

    public TriggerTableViewModel(NavigationViewModel navigation, ITriggerService service)
        : base(navigation)
    {
        Service = service;
    }
}
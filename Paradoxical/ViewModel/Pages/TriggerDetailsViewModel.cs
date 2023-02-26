using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Trigger Details";

    public ITriggerService Service { get; }

    public TriggerDetailsViewModel(NavigationViewModel navigation, ITriggerService service)
        : base(navigation)
    {
        Service = service;
    }
}
using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Trigger Details";

    public ITriggerService Service { get; }

    public TriggerDetailsViewModel(ITriggerService service)
    {
        Service = service;
    }
}
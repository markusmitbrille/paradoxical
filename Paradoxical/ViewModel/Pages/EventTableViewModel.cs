using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventTableViewModel : PageViewModelBase
{
    public override string PageName => "Events";

    public IEventService Service { get; }

    public EventTableViewModel(IEventService service)
    {
        Service = service;
    }
}
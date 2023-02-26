using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Event Details";

    public IEventService Service { get; }

    public EventDetailsViewModel(IEventService service)
    {
        Service = service;
    }
}
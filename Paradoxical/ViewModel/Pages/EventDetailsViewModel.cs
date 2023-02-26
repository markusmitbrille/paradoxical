using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Event Details";

    public IEventService Service { get; }

    public EventDetailsViewModel(NavigationViewModel navigation, IEventService service)
        : base(navigation)
    {
        Service = service;
    }
}
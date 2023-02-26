using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Event Details";

    public ApplicationViewModel App { get; }
    public IEventService Service { get; }

    public EventDetailsViewModel(ApplicationViewModel app, IEventService service)
    {
        App = app;
        Service = service;
    }
}
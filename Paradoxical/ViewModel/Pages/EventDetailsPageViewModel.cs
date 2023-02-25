using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventDetailsPageViewModel : PageViewModelBase
{
    public override string PageName => "Event Details";

    public ApplicationViewModel App { get; }
    public IEventService Service { get; }

    public EventDetailsPageViewModel(ApplicationViewModel app, IEventService service)
    {
        App = app;
        Service = service;
    }
}
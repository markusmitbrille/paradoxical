using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventTableViewModel : PageViewModelBase
{
    public override string PageName => "Events";

    public ApplicationViewModel App { get; }
    public IEventService Service { get; }

    public EventTableViewModel(ApplicationViewModel app, IEventService service)
    {
        App = app;
        Service = service;
    }
}
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EventPageViewModel : PageViewModelBase
{
    public override string PageName => "Events";

    public ApplicationViewModel App { get; }
    public IEventService Service { get; }

    public EventPageViewModel(ApplicationViewModel app, IEventService service)
    {
        App = app;
        Service = service;
    }
}
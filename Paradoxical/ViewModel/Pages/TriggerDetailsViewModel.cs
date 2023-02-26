using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Trigger Details";

    public ApplicationViewModel App { get; }
    public ITriggerService Service { get; }

    public TriggerDetailsViewModel(ApplicationViewModel app, ITriggerService service)
    {
        App = app;
        Service = service;
    }
}
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerDetailsPageViewModel : PageViewModelBase
{
    public override string PageName => "Trigger Details";

    public ApplicationViewModel App { get; }
    public ITriggerService Service { get; }

    public TriggerDetailsPageViewModel(ApplicationViewModel app, ITriggerService service)
    {
        App = app;
        Service = service;
    }
}
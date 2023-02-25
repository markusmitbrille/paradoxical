using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerPageViewModel : PageViewModelBase
{
    public override string PageName => "Triggers";

    public ApplicationViewModel App { get; }
    public ITriggerService Service { get; }

    public TriggerPageViewModel(ApplicationViewModel app, ITriggerService service)
    {
        App = app;
        Service = service;
    }
}
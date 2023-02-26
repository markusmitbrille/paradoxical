using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class TriggerTableViewModel : PageViewModelBase
{
    public override string PageName => "Triggers";

    public ApplicationViewModel App { get; }
    public ITriggerService Service { get; }

    public TriggerTableViewModel(ApplicationViewModel app, ITriggerService service)
    {
        App = app;
        Service = service;
    }
}
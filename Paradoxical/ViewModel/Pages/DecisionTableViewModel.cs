using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class DecisionTableViewModel : PageViewModelBase
{
    public override string PageName => "Decisions";

    public ApplicationViewModel App { get; }
    public IDecisionService Service { get; }

    public DecisionTableViewModel(ApplicationViewModel app, IDecisionService service)
    {
        App = app;
        Service = service;
    }
}
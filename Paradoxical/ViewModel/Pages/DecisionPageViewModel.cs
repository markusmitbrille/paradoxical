using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class DecisionPageViewModel : PageViewModelBase
{
    public override string PageName => "Decisions";

    public ApplicationViewModel App { get; }
    public IDecisionService Service { get; }

    public DecisionPageViewModel(ApplicationViewModel app, IDecisionService service)
    {
        App = app;
        Service = service;
    }
}
using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class DecisionTableViewModel : PageViewModelBase
{
    public override string PageName => "Decisions";

    public IDecisionService Service { get; }

    public DecisionTableViewModel(IDecisionService service)
    {
        Service = service;
    }
}
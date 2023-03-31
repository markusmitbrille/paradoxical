using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class DecisionTableViewModel : PageViewModel
{
    public override string PageName => "Decisions";

    public IDecisionService Service { get; }

    public DecisionTableViewModel(NavigationViewModel navigation, IDecisionService service)
        : base(navigation)
    {
        Service = service;
    }
}
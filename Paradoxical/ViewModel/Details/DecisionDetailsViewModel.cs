using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class DecisionDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Decision Details";

    public IDecisionService Service { get; }

    private DecisionViewModel? selected;
    public DecisionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public DecisionDetailsViewModel(NavigationViewModel navigation, IDecisionService service)
        : base(navigation)
    {
        Service = service;
    }
}
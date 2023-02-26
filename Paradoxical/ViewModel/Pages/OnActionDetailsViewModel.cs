using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionDetailsViewModel : PageViewModelBase
{
    public override string PageName => "On-Action Details";

    public IOnActionService Service { get; }

    public OnActionDetailsViewModel(NavigationViewModel navigation, IOnActionService service)
        : base(navigation)
    {
        Service = service;
    }
}
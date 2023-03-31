using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class OnActionTableViewModel : PageViewModel
{
    public override string PageName => "On-Actions";

    public IOnActionService Service { get; }

    public OnActionTableViewModel(NavigationViewModel navigation, IOnActionService service)
        : base(navigation)
    {
        Service = service;
    }
}
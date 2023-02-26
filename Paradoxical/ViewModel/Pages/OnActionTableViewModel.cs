using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionTableViewModel : PageViewModelBase
{
    public override string PageName => "On-Actions";

    public IOnActionService Service { get; }

    public OnActionTableViewModel(IOnActionService service)
    {
        Service = service;
    }
}
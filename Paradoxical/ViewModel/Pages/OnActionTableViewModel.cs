using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionTableViewModel : PageViewModelBase
{
    public override string PageName => "On-Actions";

    public ApplicationViewModel App { get; }
    public IOnActionService Service { get; }

    public OnActionTableViewModel(ApplicationViewModel app, IOnActionService service)
    {
        App = app;
        Service = service;
    }
}
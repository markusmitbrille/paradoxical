using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionPageViewModel : PageViewModelBase
{
    public override string PageName => "On-Actions";

    public ApplicationViewModel App { get; }
    public IOnActionService Service { get; }

    public OnActionPageViewModel(ApplicationViewModel app, IOnActionService service)
    {
        App = app;
        Service = service;
    }
}
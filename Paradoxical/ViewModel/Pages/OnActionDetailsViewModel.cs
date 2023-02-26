using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionDetailsViewModel : PageViewModelBase
{
    public override string PageName => "On-Action Details";

    public ApplicationViewModel App { get; }
    public IOnActionService Service { get; }

    public OnActionDetailsViewModel(ApplicationViewModel app, IOnActionService service)
    {
        App = app;
        Service = service;
    }
}
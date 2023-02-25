using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class OnActionDetailsPageViewModel : PageViewModelBase
{
    public override string PageName => "On-Action Details";

    public ApplicationViewModel App { get; }
    public IOnActionService Service { get; }

    public OnActionDetailsPageViewModel(ApplicationViewModel app, IOnActionService service)
    {
        App = app;
        Service = service;
    }
}
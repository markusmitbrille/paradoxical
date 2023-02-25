using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectPageViewModel : PageViewModelBase
{
    public override string PageName => "Effects";

    public ApplicationViewModel App { get; }
    public IEffectService Service { get; }

    public EffectPageViewModel(ApplicationViewModel app, IEffectService service)
    {
        App = app;
        Service = service;
    }
}
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectDetailsPageViewModel : PageViewModelBase
{
    public override string PageName => "Effect Details";

    public ApplicationViewModel App { get; }
    public IEffectService Service { get; }

    public EffectDetailsPageViewModel(ApplicationViewModel app, IEffectService service)
    {
        App = app;
        Service = service;
    }
}
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectDetailsViewModel : PageViewModelBase
{
    public override string PageName => "Effect Details";

    public ApplicationViewModel App { get; }
    public IEffectService Service { get; }

    public EffectDetailsViewModel(ApplicationViewModel app, IEffectService service)
    {
        App = app;
        Service = service;
    }
}
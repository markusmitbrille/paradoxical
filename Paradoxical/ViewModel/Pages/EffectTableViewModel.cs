using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class EffectTableViewModel : PageViewModelBase
{
    public override string PageName => "Effects";

    public ApplicationViewModel App { get; }
    public IEffectService Service { get; }

    public EffectTableViewModel(ApplicationViewModel app, IEffectService service)
    {
        App = app;
        Service = service;
    }
}
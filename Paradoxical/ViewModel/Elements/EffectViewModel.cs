using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.ViewModel;

public class EffectViewModel : ElementViewModel<Effect>
{
    public string Name
    {
        get => model.name;
        set => SetProperty(ref model.name, value);
    }

    public string Code
    {
        get => model.code;
        set => SetProperty(ref model.code, value);
    }

    public string Tooltip
    {
        get => model.tooltip;
        set => SetProperty(ref model.tooltip, value);
    }

    public bool Hidden
    {
        get => model.hidden;
        set => SetProperty(ref model.hidden, value);
    }

    public EffectViewModel(Effect model) : base(model)
    {
    }
}

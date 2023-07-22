using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.ViewModel;

public class TriggerViewModel : ElementWrapper<Trigger>
{
    public override string Kind => "trigger";

    public string? Raw
    {
        get => model.raw;
        set => SetProperty(ref model.raw, value);
    }

    public override string Name
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
}
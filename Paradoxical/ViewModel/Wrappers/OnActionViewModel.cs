using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.ViewModel;

public class OnActionViewModel : ElementWrapper<OnAction>
{
    public override string Kind => "action";

    public override string Name
    {
        get => model.name;
        set => SetProperty(ref model.name, value);
    }

    public bool Vanilla
    {
        get => model.vanilla;
        set => SetProperty(ref model.vanilla, value);
    }

    public int Chance
    {
        get => model.chance;
        set => SetProperty(ref model.chance, value);
    }

    public OnActionViewModel(OnAction model) : base(model)
    {
    }
}

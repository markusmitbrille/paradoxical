using Paradoxical.Core;
using Paradoxical.Model;

namespace Paradoxical.ViewModel;

public partial class OnActionViewModel : ViewModelBase, IElementViewModel
{
    private readonly OnAction model;
    public OnAction Model => model;

    IElement IElementViewModel.Model => Model;

    public int Id
    {
        get => model.Id;
    }

    public string Name
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

    public OnActionViewModel(OnAction model)
    {
        this.model = model;
    }
}

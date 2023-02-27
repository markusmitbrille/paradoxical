using Paradoxical.Core;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class OnActionViewModel : ViewModelBase, IModelViewModel, IElementViewModel
{
    private static readonly Dictionary<OnAction, OnActionViewModel> cache = new();
    public static OnActionViewModel Get(OnAction model)
    {
        if (cache.TryGetValue(model, out var viewModel) == false)
        {
            viewModel = new(model);
            cache.Add(model, viewModel);
        }

        return viewModel;
    }

    private readonly OnAction model;
    public OnAction Model => model;

    IModel IModelViewModel.Model => Model;
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

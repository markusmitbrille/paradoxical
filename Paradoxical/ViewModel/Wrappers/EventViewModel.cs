using Paradoxical.Core;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class EventViewModel : ViewModelBase, IModelViewModel, IElementViewModel
{
    private static readonly Dictionary<Event, EventViewModel> cache = new();
    public static EventViewModel Get(Event model)
    {
        if (cache.TryGetValue(model, out var viewModel) == false)
        {
            viewModel = new(model);
            cache.Add(model, viewModel);
        }

        return viewModel;
    }

    private readonly Event model;
    public Event Model => model;

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

    public string Title
    {
        get => model.title;
        set => SetProperty(ref model.title, value);
    }

    public string Description
    {
        get => model.description;
        set => SetProperty(ref model.description, value);
    }

    public string Theme
    {
        get => model.theme;
        set => SetProperty(ref model.theme, value);
    }

    public bool Hidden
    {
        get => model.hidden;
        set => SetProperty(ref model.hidden, value);
    }

    public int Weight
    {
        get => model.weight;
        set => SetProperty(ref model.weight, value);
    }

    public int Cooldown
    {
        get => model.cooldown;
        set => SetProperty(ref model.cooldown, value);
    }

    public EventViewModel(Event model)
    {
        this.model = model;
    }
}

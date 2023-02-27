using Paradoxical.Core;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class PortraitViewModel : ViewModelBase, IModelViewModel
{
    private static readonly Dictionary<Portrait, PortraitViewModel> cache = new();
    public static PortraitViewModel Get(Portrait model)
    {
        if (cache.TryGetValue(model, out var viewModel) == false)
        {
            viewModel = new(model);
            cache.Add(model, viewModel);
        }

        return viewModel;
    }

    private readonly Portrait model;
    public Portrait Model => model;

    IModel IModelViewModel.Model => Model;

    public int Id
    {
        get => model.Id;
    }

    public int EventId
    {
        get => model.eventId;
    }

    public string Character
    {
        get => model.character;
        set => SetProperty(ref model.character, value);
    }

    public string Animation
    {
        get => model.animation;
        set => SetProperty(ref model.animation, value);
    }

    public string OutfitTags
    {
        get => model.outfitTags;
        set => SetProperty(ref model.outfitTags, value);
    }

    public PortraitViewModel(Portrait model)
    {
        this.model = model;
    }
}
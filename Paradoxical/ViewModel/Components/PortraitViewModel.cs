using Paradoxical.Core;
using Paradoxical.Model.Components;

namespace Paradoxical.ViewModel;

public partial class PortraitViewModel : ViewModelBase, IComponentViewModel
{
    private readonly Portrait model;
    public Portrait Model => model;

    IComponent IComponentViewModel.Model => Model;

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
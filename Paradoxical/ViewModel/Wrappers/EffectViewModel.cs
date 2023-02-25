using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;

namespace Paradoxical.ViewModel;

public partial class EffectViewModel : ObservableObject
{
    private readonly Effect model;
    public Effect Model => model;

    public int Id
    {
        get => model.Id;
    }

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

    public EffectViewModel(Effect model)
    {
        this.model = model;
    }
}

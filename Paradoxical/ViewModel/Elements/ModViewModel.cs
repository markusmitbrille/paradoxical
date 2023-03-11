using Paradoxical.Core;
using Paradoxical.Model;

namespace Paradoxical.ViewModel;

public partial class ModViewModel : ViewModelBase
{
    private readonly Mod model;
    public Mod Model => model;

    public int Id
    {
        get => model.Id;
    }

    public string ModName
    {
        get => model.modName;
        set => SetProperty(ref model.modName, value);
    }

    public string ModVersion
    {
        get => model.modVersion;
        set => SetProperty(ref model.modVersion, value);
    }

    public string GameVersion
    {
        get => model.gameVersion;
        set => SetProperty(ref model.gameVersion, value);
    }

    public string Prefix
    {
        get => model.prefix;
        set => SetProperty(ref model.prefix, value);
    }

    public ModViewModel(Mod model)
    {
        this.model = model;
    }
}

using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.ViewModel;

public class ScriptViewModel : ModelWrapper<Script>
{
    public string Code
    {
        get => model.code;
        set => SetProperty(ref model.code, value);
    }

    public string Dir
    {
        get => model.dir;
        set => SetProperty(ref model.dir, value);
    }

    public string File
    {
        get => model.file;
        set => SetProperty(ref model.file, value);
    }

    public string Path => System.IO.Path.Combine(Dir, File);
}

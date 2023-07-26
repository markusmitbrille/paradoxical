using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class ScriptViewModel : ModelWrapper<Script>, IEquatable<ScriptViewModel?>
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as ScriptViewModel);
    }

    public bool Equals(ScriptViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Script>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(ScriptViewModel? left, ScriptViewModel? right)
    {
        return EqualityComparer<ScriptViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(ScriptViewModel? left, ScriptViewModel? right)
    {
        return !(left == right);
    }
}

using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class ModViewModel : ModelWrapper<Mod>, IEquatable<ModViewModel?>
{
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as ModViewModel);
    }

    public bool Equals(ModViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Mod>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(ModViewModel? left, ModViewModel? right)
    {
        return EqualityComparer<ModViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(ModViewModel? left, ModViewModel? right)
    {
        return !(left == right);
    }
}

using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EffectViewModel : ElementWrapper<Effect>, IEquatable<EffectViewModel?>
{
    public override string Kind => "effect";

    public string? Raw
    {
        get => model.raw;
        set => SetProperty(ref model.raw, value);
    }

    public override string Name
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as EffectViewModel);
    }

    public bool Equals(EffectViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Effect>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(EffectViewModel? left, EffectViewModel? right)
    {
        return EqualityComparer<EffectViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(EffectViewModel? left, EffectViewModel? right)
    {
        return !(left == right);
    }
}

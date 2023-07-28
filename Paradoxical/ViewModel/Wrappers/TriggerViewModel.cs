using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class TriggerViewModel : ElementWrapper<Trigger>, IEquatable<TriggerViewModel?>
{
    public override string Kind => "trigger";

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

    public override bool Equals(object? obj)
    {
        return Equals(obj as TriggerViewModel);
    }

    public bool Equals(TriggerViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Trigger>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(TriggerViewModel? left, TriggerViewModel? right)
    {
        return EqualityComparer<TriggerViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(TriggerViewModel? left, TriggerViewModel? right)
    {
        return !(left == right);
    }
}
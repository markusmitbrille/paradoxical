using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class LinkViewModel : ModelWrapper<Link>, IEquatable<LinkViewModel?>
{
    public int EventId
    {
        get => model.eventId;
    }

    public string Scope
    {
        get => model.scope;
        set => SetProperty(ref model.scope, value);
    }

    public int MinDays
    {
        get => model.minDays;
        set => SetProperty(ref model.minDays, value);
    }

    public int MaxDays
    {
        get => model.maxDays;
        set => SetProperty(ref model.maxDays, value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LinkViewModel);
    }

    public bool Equals(LinkViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Link>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(LinkViewModel? left, LinkViewModel? right)
    {
        return EqualityComparer<LinkViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(LinkViewModel? left, LinkViewModel? right)
    {
        return !(left == right);
    }
}

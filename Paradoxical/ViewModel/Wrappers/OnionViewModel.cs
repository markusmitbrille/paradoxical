using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class OnionViewModel : ModelWrapper<Onion>, IEquatable<OnionViewModel?>
{
    public int EventId
    {
        get => model.eventId;
    }

    public string Name
    {
        get => model.name;
        set => SetProperty(ref model.name, value);
    }

    public bool Random
    {
        get => model.random;
        set => SetProperty(ref model.random, value);
    }

    public int Weight
    {
        get => model.weight;
        set => SetProperty(ref model.weight, value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as OnionViewModel);
    }

    public bool Equals(OnionViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Onion>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(OnionViewModel? left, OnionViewModel? right)
    {
        return EqualityComparer<OnionViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(OnionViewModel? left, OnionViewModel? right)
    {
        return !(left == right);
    }
}

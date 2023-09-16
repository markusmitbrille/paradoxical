using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class PortraitViewModel : ModelWrapper<Portrait>, IEquatable<PortraitViewModel?>
{
    public int EventId
    {
        get => model.eventId;
    }

    public PortraitPosition Position
    {
        get => model.position;
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

    public override bool Equals(object? obj)
    {
        return Equals(obj as PortraitViewModel);
    }

    public bool Equals(PortraitViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Portrait>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(PortraitViewModel? left, PortraitViewModel? right)
    {
        return EqualityComparer<PortraitViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(PortraitViewModel? left, PortraitViewModel? right)
    {
        return !(left == right);
    }
}
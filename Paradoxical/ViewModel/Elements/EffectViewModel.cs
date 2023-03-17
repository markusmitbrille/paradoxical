using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EffectViewModel : ViewModelBase, IElementViewModel, IEquatable<EffectViewModel?>
{
    private readonly Effect model;
    public Effect Model => model;

    IElement IElementViewModel.Model => Model;

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

    public override bool Equals(object? obj)
    {
        return Equals(obj as EffectViewModel);
    }

    public bool Equals(EffectViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Effect>.Default.Equals(Model, other.Model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Model);
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

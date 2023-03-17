using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class OnActionViewModel : ViewModelBase, IElementViewModel, IEquatable<OnActionViewModel?>
{
    private readonly OnAction model;
    public OnAction Model => model;

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

    public bool Vanilla
    {
        get => model.vanilla;
        set => SetProperty(ref model.vanilla, value);
    }

    public int Chance
    {
        get => model.chance;
        set => SetProperty(ref model.chance, value);
    }

    public OnActionViewModel(OnAction model)
    {
        this.model = model;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as OnActionViewModel);
    }

    public bool Equals(OnActionViewModel? other)
    {
        return other is not null &&
               EqualityComparer<OnAction>.Default.Equals(Model, other.Model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Model);
    }

    public static bool operator ==(OnActionViewModel? left, OnActionViewModel? right)
    {
        return EqualityComparer<OnActionViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(OnActionViewModel? left, OnActionViewModel? right)
    {
        return !(left == right);
    }
}

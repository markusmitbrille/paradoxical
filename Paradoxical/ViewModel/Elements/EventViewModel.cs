using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EventViewModel : ViewModelBase, IElementViewModel, IEquatable<EventViewModel?>
{
    private readonly Event model;
    public Event Model => model;

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

    public string Title
    {
        get => model.title;
        set => SetProperty(ref model.title, value);
    }

    public string Description
    {
        get => model.description;
        set => SetProperty(ref model.description, value);
    }

    public string Theme
    {
        get => model.theme;
        set => SetProperty(ref model.theme, value);
    }

    public bool Hidden
    {
        get => model.hidden;
        set => SetProperty(ref model.hidden, value);
    }

    public int Weight
    {
        get => model.weight;
        set => SetProperty(ref model.weight, value);
    }

    public int Cooldown
    {
        get => model.cooldown;
        set => SetProperty(ref model.cooldown, value);
    }

    public string CustomTrigger
    {
        get => model.customTrigger;
        set => SetProperty(ref model.customTrigger, value);
    }

    public string CustomImmediateEffect
    {
        get => model.customImmediateEffect;
        set => SetProperty(ref model.customImmediateEffect, value);
    }

    public string CustomAfterEffect
    {
        get => model.customAfterEffect;
        set => SetProperty(ref model.customAfterEffect, value);
    }

    public EventViewModel(Event model)
    {
        this.model = model;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EventViewModel);
    }

    public bool Equals(EventViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Event>.Default.Equals(Model, other.Model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Model);
    }

    public static bool operator ==(EventViewModel? left, EventViewModel? right)
    {
        return EqualityComparer<EventViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(EventViewModel? left, EventViewModel? right)
    {
        return !(left == right);
    }
}

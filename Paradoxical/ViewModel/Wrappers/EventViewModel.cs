using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EventViewModel : ViewModel<Event>, IEquatable<EventViewModel?>, ISearchable
{
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

    public string Type
    {
        get => model.type;
        set => SetProperty(ref model.type, value);
    }

    public string Scope
    {
        get => model.scope;
        set => SetProperty(ref model.scope, value);
    }

    public bool Hidden
    {
        get => model.hidden;
        set => SetProperty(ref model.hidden, value);
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

    public string Filter => Name;

    public IEnumerable<Tag> Tags
    {
        get
        {
            yield return new("type", "event");
            yield return new("id", Id.ToString());
            yield return new("name", Name);
            yield return new("title", Title);
            yield return new("desc", Description);
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EventViewModel);
    }

    public bool Equals(EventViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Event>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
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

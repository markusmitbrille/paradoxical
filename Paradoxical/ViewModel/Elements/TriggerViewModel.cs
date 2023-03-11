using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class TriggerViewModel : ViewModelBase, IElementViewModel, IEquatable<TriggerViewModel?>
{
    private readonly Trigger model;
    public Trigger Model => model;

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

    public TriggerViewModel(Trigger model)
    {
        this.model = model;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TriggerViewModel);
    }

    public bool Equals(TriggerViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Trigger>.Default.Equals(Model, other.Model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Model);
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
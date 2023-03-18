using System;
using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IElementViewModel<T> where T : IElement
{
    public T Model { get; }
    public int Id { get; }
}

public abstract class ElementViewModel<T> : ViewModelBase, IElementViewModel<T>, IEquatable<ElementViewModel<T>?>
    where T : IElement
{
    protected readonly T model;
    public T Model => model;

    public int Id
    {
        get => model.Id;
    }

    protected ElementViewModel(T model)
    {
        this.model = model;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ElementViewModel<T>);
    }

    public bool Equals(ElementViewModel<T>? other)
    {
        return other is not null &&
               EqualityComparer<T>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(ElementViewModel<T>? left, ElementViewModel<T>? right)
    {
        return EqualityComparer<ElementViewModel<T>>.Default.Equals(left, right);
    }

    public static bool operator !=(ElementViewModel<T>? left, ElementViewModel<T>? right)
    {
        return !(left == right);
    }
}

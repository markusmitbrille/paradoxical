using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IModelWrapper
{
    IModel Model { get; }
    int Id { get; }
}

public interface IModelWrapper<T> where T : IModel
{
    T Model { get; }
    int Id { get; }
}

public abstract class ModelWrapper<T> : ObservableObject
    , IModelWrapper
    , IModelWrapper<T>
    , IEquatable<ModelWrapper<T>?>
    where T : IModel
{
    protected readonly T model;
    public T Model => model;

    IModel IModelWrapper.Model => Model;

    public int Id
    {
        get => model.Id;
    }

    public ModelWrapper(T model)
    {
        this.model = model;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ModelWrapper<T>);
    }

    public bool Equals(ModelWrapper<T>? other)
    {
        return other is not null &&
               EqualityComparer<T>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(ModelWrapper<T>? left, ModelWrapper<T>? right)
    {
        return EqualityComparer<ModelWrapper<T>>.Default.Equals(left, right);
    }

    public static bool operator !=(ModelWrapper<T>? left, ModelWrapper<T>? right)
    {
        return !(left == right);
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Linq;
using static Paradoxical.View.CompleteBox;

namespace Paradoxical.Core;

public interface IModelWrapper
{
    IModel Model { get; }
    int Id { get; }
}

public interface IModelWrapper<T> where T : IModel
{
    T Model { get; init; }
    int Id { get; }
}

public abstract class ModelWrapper<T> : ObservableObject
    , IModelWrapper
    , IModelWrapper<T>
    where T : IModel, new()
{
    protected readonly T model = new();
    public T Model
    {
        get => model;
        init => model = value;
    }

    IModel IModelWrapper.Model => Model;

    public int Id
    {
        get => Model.Id;
    }

    public override string ToString() => $"{GetType()} ({Id})";
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Xml.Linq;
using static Paradoxical.View.CompleteBox;

namespace Paradoxical.Core;

public interface IViewModel
{
    IModel Model { get; }
    int Id { get; }
}

public interface IViewModel<T> where T : IModel
{
    T Model { get; init; }
    int Id { get; }
}

public abstract class ViewModel<T> : ObservableObject
    , IViewModel
    , IViewModel<T>
    where T : IModel, new()
{
    protected readonly T model = new();
    public T Model
    {
        get => model;
        init => model = value;
    }

    IModel IViewModel.Model => Model;

    public int Id
    {
        get => Model.Id;
    }

    public override string ToString() => $"{GetType()} ({Id})";
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.Core;

public interface IObservableWrapper
{
    public ObservableObject Observable { get; }
}

public interface IObservableWrapper<T> where T : ObservableObject
{
    public T Observable { get; }
}

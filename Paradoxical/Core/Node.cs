using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paradoxical.Core;

public abstract class Node : ObservableObject
{
    private string header = string.Empty;
    public string Header
    {
        get => header;
        set => SetProperty(ref header, value);
    }

    public abstract IEnumerable<Node> Children { get; }
}

public abstract class ObservableNode<T> : Node, IObservableWrapper<T>, IObservableWrapper
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();

    ObservableObject IObservableWrapper.Observable => Observable;

    public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public class CollectionNode : Node
{
    private readonly ObservableCollection<Node> children = new();
    public sealed override IEnumerable<Node> Children => children;

    public void Add(Node node) => children.Add(node);
    public void Remove(Node node) => children.Remove(node);
    public void Clear() => children.Clear();
}

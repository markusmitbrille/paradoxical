using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Core;

public class NodeMap<O, N>
    where O : ObservableObject, new()
    where N : ObservableNode<O>, new()
{
    private Dictionary<O, N> Map { get; } = new();

    public N this[O observable]
    {
        get
        {
            if (Map.ContainsKey(observable) == true)
            {
                return Map[observable];
            }

            var node = new N() { Observable = observable };
            Map[observable] = node;

            return node;
        }
    }

    public IEnumerable<O> Observables => Map.Keys;
    public IEnumerable<N> Nodes => Map.Values;

    public NodeMap()
    {
    }

    public NodeMap(IEnumerable<O> observables)
    {
        Map = observables.ToDictionary(observable => observable, observable => new N() { Observable = observable });
    }

    public void Remove(O observable)
    {
        Map.Remove(observable);
    }
}

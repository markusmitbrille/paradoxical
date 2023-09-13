using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paradoxical.Core;

public class Node : ObservableObject
{
    public ObservableCollection<Node> Children { get; } = new();

    private string header = string.Empty;
    public string Header
    {
        get => header;
        set => SetProperty(ref header, value);
    }

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    private bool isExpanded = false;
    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }

    public void Select() => IsSelected = true;

    public void Expand() => IsExpanded = true;
    public void Collapse() => IsExpanded = false;
}

public abstract class ObservableNode<T> : Node, IObservableWrapper<T>, IObservableWrapper
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();
    ObservableObject IObservableWrapper.Observable => Observable;
}

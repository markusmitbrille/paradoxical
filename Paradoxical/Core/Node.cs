using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Paradoxical.Core;

public abstract partial class Node : ObservableObject
{
    public abstract IEnumerable<Node> Children { get; }

    public IEnumerable<Node> Descendants
    {
        get
        {
            var descendants = new List<Node>(Children);
            for (int i = 0; i < descendants.Count; i++)
            {
                var descendant = descendants[i];
                descendants.AddRange(descendant.Children);
            }

            return descendants;
        }
    }

    [GeneratedRegex(@"(?<path>\w+)(?>/(?<subpath>\w+(?>/\w+)?))?")]
    private static partial Regex GetPathRegex();
    private static Regex PathRegex => GetPathRegex();

    public Node? this[string index]
    {
        get
        {
            var match = PathRegex.Match(index);

            if (match.Success == false)
            { return null; }

            if (match.Groups["path"].Success == false)
            { return null; }

            string path = match.Groups["path"].Value;
            Node? child = Children.FirstOrDefault(node => node.Path == path);

            if (child == null)
            { return null; }

            if (match.Groups["subpath"].Success == false)
            { return child; }

            string subpath = match.Groups["subpath"].Value;
            return child[subpath];
        }
    }

    public abstract string Path { get; }
    public abstract string Header { get; }

    private bool isSelected = false;
    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    public void Select() => IsSelected = true;
    public void Unselect() => IsSelected = true;

    private bool isExpanded = false;
    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }

    public void Expand() => IsExpanded = true;
    public void Collapse() => IsExpanded = false;
}

public sealed class CollectionNode : Node
{
    private readonly ObservableCollection<Node> children = new();
    public override IEnumerable<Node> Children => children;

    public void Add(Node child) => children.Add(child);
    public void Remove(Node child) => children.Remove(child);

    public void RemoveAll(Predicate<Node> match) => children.RemoveAll(match);

    public string Name { get; init; } = string.Empty;

    public override string Path => Name;
    public override string Header => Name;
}

public abstract class ObservableNode<T> : Node, IObservableWrapper<T>, IObservableWrapper
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();
    ObservableObject IObservableWrapper.Observable => Observable;

    public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

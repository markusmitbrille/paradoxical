using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
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
    private Node? parent;
    public Node? Parent
    {
        get => parent;
        set => SetProperty(ref parent, value);
    }

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

    public void Select()
    {
        var parent = this.parent;
        while (parent != null)
        {
            parent.Expand();
            parent = parent.parent;
        }

        IsSelected = true;
    }

    public void Unselect() => IsSelected = true;

    private bool isExpanded = false;
    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }

    public void Expand() => IsExpanded = true;
    public void Collapse() => IsExpanded = false;

    public void CollapseChildren()
    {
        foreach (var node in Children)
        {
            node.Collapse();
        }
    }

    public void CollapseDescendants()
    {
        foreach (var node in Descendants)
        {
            node.Collapse();
        }
    }

    public void CollapseSiblings()
    {
        if (Parent == null)
        { return; }

        var siblings = Parent.Children.Except(new Node[] { this });
        foreach (var node in siblings)
        {
            node.Collapse();
        }
    }
}

public sealed class CollectionNode : Node
{
    private readonly ObservableCollection<Node> children = new();
    public override IEnumerable<Node> Children => children;

    public void Add(Node node)
    {
        node.Parent = this;
        children.Add(node);
    }

    public void Remove(Node node)
    {
        node.Parent = null;
        children.Remove(node);
    }

    public void RemoveAll(Predicate<Node> match)
    {
        var nodes = children.Where(item => match(item)).ToList();
        foreach (var node in nodes)
        {
            node.Parent = null;
            children.Remove(node);
        }
    }

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

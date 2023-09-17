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

public interface INode
{
    INode? Parent { get; set; }
    IEnumerable<INode> Children { get; }
    IEnumerable<INode> Descendants { get; }

    INode? this[string index] { get; }

    string Header { get; }
    string Path { get; }

    bool IsExpanded { get; set; }
    bool IsSelected { get; set; }

    void Select();
    void Focus();
    void Unselect();
    void Expand();
    void Collapse();
    void CollapseChildren();
    void CollapseDescendants();
    void CollapseSiblings();
}

public abstract partial class Node : ObservableObject, INode
{
    private INode? parent;
    public INode? Parent
    {
        get => parent;
        set => SetProperty(ref parent, value);
    }

    public abstract IEnumerable<INode> Children { get; }

    public IEnumerable<INode> Descendants
    {
        get
        {
            var descendants = new List<INode>(Children);
            for (int i = 0; i < descendants.Count; i++)
            {
                var descendant = descendants[i];
                yield return descendant;

                descendants.AddRange(descendant.Children);
            }
        }
    }

    [GeneratedRegex(@"(?<path>\w+)(?>/(?<subpath>\w+(?>/\w+)?))?")]
    private static partial Regex GetPathRegex();
    private static Regex PathRegex => GetPathRegex();

    public INode? this[string index]
    {
        get
        {
            var match = PathRegex.Match(index);

            if (match.Success == false)
            { return null; }

            if (match.Groups["path"].Success == false)
            { return null; }

            string path = match.Groups["path"].Value;
            INode? child = Children.FirstOrDefault(node => node.Path == path);

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

    private bool isExpanded = false;
    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }

    public void Select()
    {
        var parent = this.parent;
        while (parent != null)
        {
            parent.Expand();
            parent = parent.Parent;
        }

        IsSelected = true;
    }

    public void Unselect() => IsSelected = true;

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

        var siblings = Parent.Children.Except(new INode[] { this });
        foreach (var node in siblings)
        {
            node.Collapse();
        }
    }

    public void Focus()
    {
        INode root = this;

        var parent = this.parent;
        while (parent != null)
        {
            root = parent;
            parent = parent.Parent;
        }

        root.CollapseDescendants();

        Select();
        Expand();
    }
}

public sealed class CollectionNode : Node
{
    private readonly ObservableCollection<INode> children = new();
    public override IEnumerable<INode> Children => children;

    public void Add(INode node)
    {
        node.Parent = this;
        children.Add(node);
    }

    public void Remove(INode node)
    {
        node.Parent = null;
        children.Remove(node);
    }

    public void RemoveAll(Predicate<INode> match)
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

public interface IObservableNode : INode
{
    public ObservableObject Observable { get; }
}

public interface IObservableNode<T> : INode
    where T : ObservableObject, new()
{
    public T Observable { get; init; }
}

public abstract class ObservableNode<T> : Node, IObservableNode<T>, IObservableNode
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();
    ObservableObject IObservableNode.Observable => Observable;

    public override string Path => Observable.ToString() ?? string.Empty;
    public override string Header => Observable.ToString() ?? string.Empty;

    public override IEnumerable<INode> Children => Enumerable.Empty<INode>();
}

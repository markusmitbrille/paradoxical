﻿using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Paradoxical.Core;

public interface INode
{
    INode? Parent { get; set; }
    INode Root { get; }
    IEnumerable<INode> Children { get; }
    IEnumerable<INode> Descendants { get; }

    INode? this[string index] { get; }

    string Header { get; }
    string Path { get; }

    bool IsExpanded { get; set; }
    bool IsSelected { get; set; }

    INode Select();
    INode Highlight();
    INode Focus();
    INode Unselect();
    INode UnselectDescendants();
    INode Expand();
    INode ExpandAncestors();
    INode Collapse();
    INode CollapseChildren();
    INode CollapseDescendants();
    INode CollapseSiblings();

    void AddSortDescription(SortDescription item);
    void RemoveSortDescription(SortDescription item);
    void Refresh();
}

public abstract partial class Node : ObservableObject, INode
{
    private INode? parent;
    public INode? Parent
    {
        get => parent;
        set => SetProperty(ref parent, value);
    }

    public INode Root
    {
        get
        {
            INode root = this;

            var parent = this.parent;
            while (parent != null)
            {
                root = parent;
                parent = parent.Parent;
            }

            return root;
        }
    }

    public abstract IEnumerable<INode> Children { get; }

    public IEnumerable<INode> Ancestors
    {
        get
        {
            var parent = this.parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }
    }

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

    public INode Select()
    {
        ExpandAncestors();

        IsSelected = true;

        return this;
    }

    public INode Highlight()
    {
        CollapseSiblings();

        ExpandAncestors();

        IsSelected = true;
        IsExpanded = true;

        return this;
    }

    public INode Focus()
    {
        Root.CollapseDescendants();

        ExpandAncestors();

        IsSelected = true;
        IsExpanded = true;

        return this;
    }

    public INode Unselect()
    {
        IsSelected = false;

        return this;
    }

    public INode UnselectDescendants()
    {
        foreach (var node in Descendants)
        {
            node.Unselect();
        }

        return this;
    }

    public INode Expand()
    {
        IsExpanded = true;

        return this;
    }

    public INode ExpandAncestors()
    {
        foreach (var node in Ancestors)
        {
            node.Expand();
        }

        return this;
    }

    public INode Collapse()
    {
        IsExpanded = false;

        return this;
    }

    public INode CollapseChildren()
    {
        foreach (var node in Children)
        {
            node.Collapse();
        }

        return this;
    }

    public INode CollapseDescendants()
    {
        foreach (var node in Descendants)
        {
            node.Collapse();
        }

        return this;
    }

    public INode CollapseSiblings()
    {
        if (Parent == null)
        { return this; }

        var siblings = Parent.Children.Except(new INode[] { this });
        foreach (var node in siblings)
        {
            node.Collapse();
        }

        return this;
    }

    private ICollectionView CollectionView => CollectionViewSource.GetDefaultView(Children);

    public void AddSortDescription(SortDescription item)
    {
        CollectionView.SortDescriptions.Add(item);
    }

    public void RemoveSortDescription(SortDescription item)
    {
        CollectionView.SortDescriptions.Remove(item);
    }

    public void Refresh()
    {
        CollectionView.Refresh();
    }

    public void RefreshDescendants()
    {
        foreach (var node in Descendants)
        {
            node.Refresh();
        }
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

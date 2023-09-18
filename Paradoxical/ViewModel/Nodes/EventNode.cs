﻿using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class EventNode : ObservableNode<EventViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DuplicateCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }
}

public sealed class EventBranch : EventNode
{
    public CollectionNode PortraitNodes { get; } = new() { Name = "Portraits", IsExpanded = false };
    public CollectionNode OptionNodes { get; } = new() { Name = "Options", IsExpanded = true };
    public CollectionNode OnionNodes { get; } = new() { Name = "On-Actions", IsExpanded = false };

    public EventBranch()
    {
        PortraitNodes.Parent = this;
        OptionNodes.Parent = this;
        OnionNodes.Parent = this;
    }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return PortraitNodes;
            yield return OptionNodes;
            yield return OnionNodes;
        }
    }
}

public sealed class EventLeaf : EventNode
{
}

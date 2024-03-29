﻿using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;
using System.ComponentModel;

namespace Paradoxical.ViewModel;

public abstract class EventNode : ObservableNode<EventViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DuplicateCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? LinkCommand { get; set; }
    public RelayCommand<object>? CreateEventCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }
}

public sealed class EventBranch : EventNode
{
    public CollectionNode PortraitNodes { get; } = new() { Name = "Portraits", IsExpanded = false };
    public CollectionNode OptionNodes { get; } = new() { Name = "Options", IsExpanded = true };
    public CollectionNode LinkNodes { get; } = new() { Name = "Links", IsExpanded = true };
    public CollectionNode OnionNodes { get; } = new() { Name = "On-Actions", IsExpanded = true };

    public EventBranch()
    {
        PortraitNodes.Parent = this;
        OptionNodes.Parent = this;
        OnionNodes.Parent = this;

        OptionNodes.AddSortDescription(new($"{nameof(OptionNode.Observable)}.{nameof(OptionViewModel.Priority)}", ListSortDirection.Ascending));
        OptionNodes.AddSortDescription(new($"{nameof(OptionNode.Observable)}.{nameof(OptionViewModel.Name)}", ListSortDirection.Ascending));

        OnionNodes.AddSortDescription(new($"{nameof(OnionNode.Observable)}.{nameof(OnionViewModel.Name)}", ListSortDirection.Ascending));
    }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return PortraitNodes;
            yield return OptionNodes;
            yield return LinkNodes;
            yield return OnionNodes;
        }
    }
}

public sealed class EventLeaf : EventNode
{
}

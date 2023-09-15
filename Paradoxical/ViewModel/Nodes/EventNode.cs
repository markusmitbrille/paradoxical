using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class EventNode : ObservableNode<EventViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }

    public AsyncRelayCommand<object>? AddTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddImmediateEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveImmediateEffectCommand { get; set; }

    public AsyncRelayCommand<object>? AddAfterEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveAfterEffectCommand { get; set; }
}

public sealed class EventBranch : EventNode
{
    public CollectionNode PortraitNodes { get; } = new() { Name = "Portraits", IsExpanded = true };
    public CollectionNode OptionNodes { get; } = new() { Name = "Options", IsExpanded = true };
    public CollectionNode OnionNodes { get; } = new() { Name = "On-Actions", IsExpanded = true };

    public CollectionNode TriggerNodes { get; } = new() { Name = "Trigger", IsExpanded = true };
    public CollectionNode ImmediateEffectNodes { get; } = new() { Name = "Immediate", IsExpanded = true };
    public CollectionNode AfterEffectNodes { get; } = new() { Name = "After", IsExpanded = true };

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return PortraitNodes;
            yield return OptionNodes;
            yield return OnionNodes;

            yield return TriggerNodes;
            yield return ImmediateEffectNodes;
            yield return AfterEffectNodes;
        }
    }
}

public sealed class EventLeaf : EventNode
{
}

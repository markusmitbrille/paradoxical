using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class DecisionNode : ObservableNode<DecisionViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public AsyncRelayCommand<object>? AddShownTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveShownTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddFailureTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveFailureTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddValidTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveValidTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveEffectCommand { get; set; }
}

public sealed class DecisionBranch : DecisionNode
{
    public CollectionNode ShownTriggerNodes { get; } = new() { Name = "Shown", IsExpanded = true };
    public CollectionNode FailureTriggerNodes { get; } = new() { Name = "Failure", IsExpanded = true };
    public CollectionNode ValidTriggerNodes { get; } = new() { Name = "Valid", IsExpanded = true };
    public CollectionNode EffectNodes { get; } = new() { Name = "Effects", IsExpanded = true };

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return ShownTriggerNodes;
            yield return FailureTriggerNodes;
            yield return ValidTriggerNodes;
            yield return EffectNodes;
        }
    }
}

public sealed class DecisionLeaf : DecisionNode
{
}

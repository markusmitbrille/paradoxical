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
    public CollectionNode ShownTriggerNodes { get; } = new() { Name = "Shown Triggers" };
    public CollectionNode FailureTriggerNodes { get; } = new() { Name = "Failure Triggers" };
    public CollectionNode ValidTriggerNodes { get; } = new() { Name = "Valid Triggers" };
    public CollectionNode EffectNodes { get; } = new() { Name = "Decision Effects" };

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

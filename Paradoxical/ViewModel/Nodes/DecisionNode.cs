using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class DecisionNode : ObservableNode<DecisionViewModel>
{
    public CollectionNode ShownTriggerNodes { get; } = new();
    public CollectionNode FailureTriggerNodes { get; } = new();
    public CollectionNode ValidTriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public RelayCommand<object>? AddShownTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveShownTriggerCommand { get; set; }

    public RelayCommand<object>? AddFailureTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveFailureTriggerCommand { get; set; }

    public RelayCommand<object>? AddValidTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveValidTriggerCommand { get; set; }

    public RelayCommand<object>? AddEffectCommand { get; set; }
    public RelayCommand<object>? RemoveEffectCommand { get; set; }

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

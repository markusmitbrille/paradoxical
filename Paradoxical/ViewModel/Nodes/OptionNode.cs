using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class OptionNode : ObservableNode<OptionViewModel>
{
    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public RelayCommand<object>? AddTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveTriggerCommand { get; set; }

    public RelayCommand<object>? AddEffectCommand { get; set; }
    public RelayCommand<object>? RemoveEffectCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return TriggerNodes;
            yield return EffectNodes;
        }
    }
}

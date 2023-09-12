using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class ModNode : ObservableNode<ModViewModel>
{
    public CollectionNode ScriptNodes { get; } = new();
    public CollectionNode EventNodes { get; } = new();
    public CollectionNode DecisionNodes { get; } = new();
    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public RelayCommand<object>? EditCommand { get; set; }

    public RelayCommand? CreateScriptCommand { get; set; }
    public RelayCommand? CreateEventCommand { get; set; }
    public RelayCommand? CreateDecisionCommand { get; set; }
    public RelayCommand? CreateTriggerCommand { get; set; }
    public RelayCommand? CreateEffectCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return ScriptNodes;
            yield return EventNodes;
            yield return DecisionNodes;
            yield return TriggerNodes;
            yield return EffectNodes;
        }
    }
}

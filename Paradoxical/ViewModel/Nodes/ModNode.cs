using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class ModNode : ObservableNode<ModViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.ModName.ToString();

    public RelayCommand<object>? EditCommand { get; set; }

    public RelayCommand? CreateScriptCommand { get; set; }
    public RelayCommand? CreateEventCommand { get; set; }
    public RelayCommand? CreateDecisionCommand { get; set; }
    public RelayCommand? CreateTriggerCommand { get; set; }
    public RelayCommand? CreateEffectCommand { get; set; }
}

public sealed class ModBranch : ModNode
{
    public CollectionNode ScriptNodes { get; } = new() { Name = "Scripts" };
    public CollectionNode EventNodes { get; } = new() { Name = "Events" };
    public CollectionNode DecisionNodes { get; } = new() { Name = "Decisions" };
    public CollectionNode TriggerNodes { get; } = new() { Name = "Triggers" };
    public CollectionNode EffectNodes { get; } = new() { Name = "Effects" };

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

public sealed class ModLeaf : ModNode
{
}

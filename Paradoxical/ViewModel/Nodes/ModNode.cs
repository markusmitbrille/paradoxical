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
}

public sealed class ModBranch : ModNode
{
    public CollectionNode ScriptNodes { get; } = new() { Name = "Scripts", IsExpanded = true };
    public CollectionNode EventNodes { get; } = new() { Name = "Events", IsExpanded = true };
    public CollectionNode DecisionNodes { get; } = new() { Name = "Decisions", IsExpanded = true };

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return ScriptNodes;
            yield return EventNodes;
            yield return DecisionNodes;
        }
    }
}

public sealed class ModLeaf : ModNode
{
}

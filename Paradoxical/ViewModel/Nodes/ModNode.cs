using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;
using System.ComponentModel;

namespace Paradoxical.ViewModel;

public abstract class ModNode : ObservableNode<ModViewModel>
{
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

    public ModBranch()
    {
        ScriptNodes.Parent = this;
        EventNodes.Parent = this;
        DecisionNodes.Parent = this;

        ScriptNodes.AddSortDescription(new($"{nameof(ScriptNode.Observable)}.{nameof(ScriptViewModel.Dir)}", ListSortDirection.Ascending));
        ScriptNodes.AddSortDescription(new($"{nameof(ScriptNode.Observable)}.{nameof(ScriptViewModel.File)}", ListSortDirection.Ascending));

        EventNodes.AddSortDescription(new($"{nameof(EventNode.Observable)}.{nameof(EventViewModel.Name)}", ListSortDirection.Ascending));

        DecisionNodes.AddSortDescription(new($"{nameof(DecisionNode.Observable)}.{nameof(DecisionViewModel.Name)}", ListSortDirection.Ascending));
    }

    public override IEnumerable<INode> Children
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

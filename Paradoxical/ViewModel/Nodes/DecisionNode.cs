using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class DecisionNode : ObservableNode<DecisionViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DuplicateCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? LinkCommand { get; set; }
    public RelayCommand<object>? CreateEventCommand { get; set; }
}

public sealed class DecisionBranch : DecisionNode
{
    public CollectionNode LinkNodes { get; } = new() { Name = "Links", IsExpanded = true };

    public DecisionBranch()
    {
        LinkNodes.Parent = this;
    }

    public override IEnumerable<INode> Children
    {
        get
        {
            yield return LinkNodes;
        }
    }
}

public sealed class DecisionLeaf : DecisionNode
{
}

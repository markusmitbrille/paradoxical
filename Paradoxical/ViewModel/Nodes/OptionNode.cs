using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class OptionNode : ObservableNode<OptionViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? LinkCommand { get; set; }
}

public sealed class OptionBranch : OptionNode
{
    public CollectionNode LinkNodes { get; } = new() { Name = "Links", IsExpanded = true };

    public OptionBranch()
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

public sealed class OptionLeaf : OptionNode
{
}

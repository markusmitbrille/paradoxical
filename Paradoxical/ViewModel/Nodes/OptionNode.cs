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
    public CollectionNode LinkNodes { get; } = new() { Name = "Triggered Events", IsExpanded = true };

    public OptionBranch()
    {
        LinkNodes.Parent = this;
    }

    public override IEnumerable<Node> Children
    {
        get
        {
            return LinkNodes.Children;
        }
    }
}

public sealed class OptionLeaf : OptionNode
{
}

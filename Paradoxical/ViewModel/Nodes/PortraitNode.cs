using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public abstract class PortraitNode : ObservableNode<PortraitViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
}

public sealed class PortraitBranch : PortraitNode
{
}

public sealed class PortraitLeaf : PortraitNode
{
}

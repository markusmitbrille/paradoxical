using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public abstract class ScriptNode : ObservableNode<ScriptViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class ScriptBranch : ScriptNode
{
}

public sealed class ScriptLeaf : ScriptNode
{
}

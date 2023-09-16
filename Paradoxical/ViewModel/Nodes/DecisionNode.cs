using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class DecisionNode : ObservableNode<DecisionViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class DecisionBranch : DecisionNode
{
}

public sealed class DecisionLeaf : DecisionNode
{
}

﻿using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public abstract class OnionNode : ObservableNode<OnionViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DuplicateCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class OnionBranch : OnionNode
{
}

public sealed class OnionLeaf : OnionNode
{
}

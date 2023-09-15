﻿using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public abstract class ScriptNode : ObservableNode<ScriptViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.File.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class ScriptBranch : ScriptNode
{
}

public sealed class ScriptLeaf : ScriptNode
{
}
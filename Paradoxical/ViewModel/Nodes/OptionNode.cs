using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public abstract class OptionNode : ObservableNode<OptionViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Title.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class OptionBranch : OptionNode
{
}

public sealed class OptionLeaf : OptionNode
{
}

using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public abstract class PortraitNode : ObservableNode<PortraitViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Position.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
}

public sealed class PortraitBranch : PortraitNode
{
}

public sealed class PortraitLeaf : PortraitNode
{
}

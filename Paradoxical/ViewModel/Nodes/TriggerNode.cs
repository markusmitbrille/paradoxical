using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Xml.Linq;

namespace Paradoxical.ViewModel;

public abstract class TriggerNode : ObservableNode<TriggerViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public sealed class TriggerBranch : TriggerNode
{
}

public sealed class TriggerLeaf : TriggerNode
{
}
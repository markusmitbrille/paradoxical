using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public class PortraitNode : ObservableNode<PortraitViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Position.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

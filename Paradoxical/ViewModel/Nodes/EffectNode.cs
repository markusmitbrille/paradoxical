using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public class EffectNode : ObservableNode<EffectViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}

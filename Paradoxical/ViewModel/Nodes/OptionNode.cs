using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class OptionNode : ObservableNode<OptionViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Title.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public AsyncRelayCommand<object>? AddTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveEffectCommand { get; set; }
}

using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class DecisionNode : ObservableNode<DecisionViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.Name.ToString();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public AsyncRelayCommand<object>? AddShownTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveShownTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddFailureTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveFailureTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddValidTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveValidTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveEffectCommand { get; set; }
}

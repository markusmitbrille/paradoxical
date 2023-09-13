using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EventNode : ObservableNode<EventViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreatePortraitCommand { get; set; }
    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }

    public AsyncRelayCommand<object>? AddTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddImmediateEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveImmediateEffectCommand { get; set; }

    public AsyncRelayCommand<object>? AddAfterEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveAfterEffectCommand { get; set; }
}

using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class EventNode : ObservableNode<EventViewModel>
{
    public CollectionNode PortraitNodes { get; } = new();
    public CollectionNode OptionNodes { get; } = new();
    public CollectionNode OnionNodes { get; } = new();

    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode ImmediateEffectNodes { get; } = new();
    public CollectionNode AfterEffectNodes { get; } = new();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreatePortraitCommand { get; set; }
    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }

    public RelayCommand<object>? AddTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveTriggerCommand { get; set; }

    public RelayCommand<object>? AddImmediateEffectCommand { get; set; }
    public RelayCommand<object>? RemoveImmediateEffectCommand { get; set; }

    public RelayCommand<object>? AddAfterEffectCommand { get; set; }
    public RelayCommand<object>? RemoveAfterEffectCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return PortraitNodes;
            yield return OptionNodes;
            yield return OnionNodes;

            yield return TriggerNodes;
            yield return ImmediateEffectNodes;
            yield return AfterEffectNodes;
        }
    }
}

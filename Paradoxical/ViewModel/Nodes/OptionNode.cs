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

    public AsyncRelayCommand<object>? AddTriggerCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveTriggerCommand { get; set; }

    public AsyncRelayCommand<object>? AddEffectCommand { get; set; }
    public AsyncRelayCommand<object>? RemoveEffectCommand { get; set; }
}

public sealed class OptionBranch : OptionNode
{
    public CollectionNode TriggerNodes { get; } = new() { Name = "Option Triggers" };
    public CollectionNode EffectNodes { get; } = new() { Name = "Option Effects" };

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return TriggerNodes;
            yield return EffectNodes;
        }
    }
}

public sealed class OptionLeaf : OptionNode
{
}

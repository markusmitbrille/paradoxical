using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class ModNode : ObservableNode<ModViewModel>
{
    public override string Path => Observable.Id.ToString();
    public override string Header => Observable.ModName.ToString();

    public RelayCommand<object>? EditCommand { get; set; }

    public RelayCommand? CreateScriptCommand { get; set; }
    public RelayCommand? CreateEventCommand { get; set; }
    public RelayCommand? CreateDecisionCommand { get; set; }
    public RelayCommand? CreateTriggerCommand { get; set; }
    public RelayCommand? CreateEffectCommand { get; set; }
}

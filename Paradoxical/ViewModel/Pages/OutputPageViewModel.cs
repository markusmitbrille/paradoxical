using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Entities;
using Paradoxical.Services;
using Paradoxical.Services.Entities;
using Paradoxical.Services.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Paradoxical.ViewModel;

public class OutputPageViewModel : PageViewModel
{
    public override string PageName => "Output";

    public IBuildService BuildService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private string eventsFileOutput = string.Empty;
    public string EventsFileOutput
    {
        get => eventsFileOutput;
        set => SetProperty(ref eventsFileOutput, value);
    }

    private string decisionsFileOutput = string.Empty;
    public string DecisionsFileOutput
    {
        get => decisionsFileOutput;
        set => SetProperty(ref decisionsFileOutput, value);
    }

    private string triggersFileOutput = string.Empty;
    public string TriggersFileOutput
    {
        get => triggersFileOutput;
        set => SetProperty(ref triggersFileOutput, value);
    }

    private string effectsFileOutput = string.Empty;
    public string EffectsFileOutput
    {
        get => effectsFileOutput;
        set => SetProperty(ref effectsFileOutput, value);
    }

    private string onionsFileOutput = string.Empty;
    public string OnionsFileOutput
    {
        get => onionsFileOutput;
        set => SetProperty(ref onionsFileOutput, value);
    }

    private string locFileOutput = string.Empty;
    public string LocFileOutput
    {
        get => locFileOutput;
        set => SetProperty(ref locFileOutput, value);
    }

    public OutputPageViewModel(
        IShell shell,
        IMediatorService mediator,
        IBuildService buildService)
        : base(shell, mediator)
    {
        BuildService = buildService;
    }

    public override void OnNavigatedTo()
    {
        Reload();
    }

    public void Load()
    {
        using (StringWriter writer = new())
        {
            BuildService.WriteEventsFile(writer);
            EventsFileOutput = writer.ToString();
        }
        using (StringWriter writer = new())
        {
            BuildService.WriteDecisionsFile(writer);
            DecisionsFileOutput = writer.ToString();
        }
        using (StringWriter writer = new())
        {
            BuildService.WriteOnionsFile(writer);
            OnionsFileOutput = writer.ToString();
        }
        using (StringWriter writer = new())
        {
            BuildService.WriteLocFile(writer);
            LocFileOutput = writer.ToString();
        }
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        Load();
    }
}

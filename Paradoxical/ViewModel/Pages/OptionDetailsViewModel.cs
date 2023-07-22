using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class OptionDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Option Details";

    public IFinder Finder { get; }

    public IModService ModService { get; }
    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private OptionViewModel? selected;
    public OptionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel> Triggers => triggers ??= new();

    private ObservableCollection<EffectViewModel>? effects;
    public ObservableCollection<EffectViewModel> Effects => effects ??= new();

    private ObservableCollection<EventViewModel>? allEvents;
    public ObservableCollection<EventViewModel> AllEvents => allEvents ??= new();

    public int? TriggeredEventId
    {
        get => Selected?.TriggeredEventId;
        set
        {
            if (Selected == null)
            { return; }

            OnPropertyChanging();
            Selected.TriggeredEventId = value;
            OnPropertyChanged();

            EditTriggeredEventCommand.NotifyCanExecuteChanged();
            RemoveTriggeredEventCommand.NotifyCanExecuteChanged();
        }
    }

    public OptionDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IFinder finder,
        IModService modService,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IEventService eventService,
        IPortraitService portraitService)
        : base(shell, mediator)
    {
        Finder = finder;

        ModService = modService;
        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
        EventService = eventService;
        PortraitService = portraitService;
    }

    public override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        Mediator.Unregister<SaveMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    void IMessageHandler<ShutdownMessage>.Handle(ShutdownMessage message)
    {
        Save();
    }

    public void Load(Option model)
    {
        var selected = OptionService.Get(model);

        var owner = OptionService.GetEvent(model);

        var triggers = OptionService.GetTriggers(selected)
            .Select(model => new TriggerViewModel() { Model = model });

        var effects = OptionService.GetEffects(selected)
            .Select(model => new EffectViewModel() { Model = model });

        var allEvents = EventService.Get()
            .Select(model => new EventViewModel() { Model = model });

        Selected = new() { Model = selected };

        Triggers.Clear();
        Triggers.AddRange(triggers);

        Effects.Clear();
        Effects.AddRange(effects);

        AllEvents.Clear();
        AllEvents.AddRange(allEvents);

        LoadRaw();
    }

    private void LoadRaw()
    {
        if (Selected == null)
        { return; }

        if (Selected.Raw == null)
        {
            OverrideRaw = false;

            // regenerate view model raw
            Raw = GenerateRaw();
        }
        else
        {
            OverrideRaw = true;

            // set view model raw to model and wrapper raw
            Raw = Selected.Raw;
        }
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        SaveRaw();

        OptionService.Update(Selected.Model);
    }

    private void SaveRaw()
    {
        if (Selected == null)
        { return; }

        if (OverrideRaw == true)
        {
            // overwrite model raw
            Selected.Raw = Raw;
        }
        else
        {
            // regenerate view model raw
            Raw = GenerateRaw();

            // clear model and wrapper raw
            Selected.Raw = null;
        }
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        if (Selected == null)
        { return; }

        Option model = new() { EventId = Selected.EventId };

        OptionService.Insert(model);

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Option model = new(Selected.Model);
        OptionService.Insert(model);

        var triggers = OptionService.GetTriggers(Selected.Model);
        foreach (var trigger in triggers)
        {
            OptionService.AddTrigger(model, trigger);
        }

        var effects = OptionService.GetEffects(Selected.Model);
        foreach (var effect in effects)
        {
            OptionService.AddEffect(model, effect);
        }

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        Event owner = OptionService.GetEvent(Selected.Model);

        OptionService.Delete(Selected.Model);

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(owner);

        var historyPages = Shell.PageHistory.OfType<OptionDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<OptionDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }

    private RelayCommand<OptionViewModel>? editPreviousOptionCommand;
    public RelayCommand<OptionViewModel> EditPreviousOptionCommand => editPreviousOptionCommand ??= new(EditPreviousOption, CanEditPreviousOption);

    private void EditPreviousOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return; }

        var owner = OptionService.GetEvent(observable.Model);

        var siblings = EventService.GetOptions(owner).ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) - 1;
        if (index < 0)
        {
            return;
        }

        var model = siblings[index];

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditPreviousOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return false; }

        var owner = OptionService.GetEvent(observable.Model);

        var siblings = EventService.GetOptions(owner).ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) - 1;

        return index >= 0;
    }

    private RelayCommand<OptionViewModel>? editNextOptionCommand;
    public RelayCommand<OptionViewModel> EditNextOptionCommand => editNextOptionCommand ??= new(EditNextOption, CanEditNextOption);

    private void EditNextOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return; }

        var owner = OptionService.GetEvent(observable.Model);

        var siblings = EventService.GetOptions(owner).ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) + 1;
        if (index >= siblings.Count)
        {
            return;
        }

        var model = siblings[index];

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditNextOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return false; }

        var owner = OptionService.GetEvent(observable.Model);

        var siblings = EventService.GetOptions(owner).ToList();
        siblings.Sort();

        int index = siblings.IndexOf(observable.Model) + 1;

        return index < siblings.Count;
    }

    #region Raw

    private bool? overrideRaw = null;
    public bool? OverrideRaw
    {
        get => overrideRaw;
        set => SetProperty(ref overrideRaw, value);
    }

    private string raw = string.Empty;
    public string Raw
    {
        get => raw;
        set => SetProperty(ref raw, value);
    }

    private RelayCommand<bool?>? toggleOverrideRawCommand;
    public RelayCommand<bool?> ToggleOverrideRawCommand => toggleOverrideRawCommand ??= new(ToggleOverrideRaw);

    private void ToggleOverrideRaw(bool? isChecked)
    {
        if (isChecked == true)
        {
            ToggleOverrideRawOn();
        }
        if (isChecked == false)
        {
            ToggleOverrideRawOff();
        }
    }

    private void ToggleOverrideRawOn()
    {
        if (Selected == null)
        { return; }

        Raw = GenerateRaw();
        Selected.Raw = Raw;
    }

    private void ToggleOverrideRawOff()
    {
        if (Selected == null)
        { return; }

        Raw = GenerateRaw();
        Selected.Raw = null;
    }

    private string GenerateRaw()
    {
        if (Selected == null)
        { return string.Empty; }

        using StringWriter writer = new();

        Selected.Model.Write(writer, ModService, OptionService);

        return writer.ToString();
    }

    #endregion

    #region Flow Commands

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger);

    private void CreateTrigger()
    {
        if (Selected == null)
        { return; }

        Option owner = Selected.Model;
        Trigger relation = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

        TriggerService.Insert(relation);
        OptionService.AddTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        Triggers.Add(observable);
    }

    private AsyncRelayCommand? addTriggerCommand;
    public AsyncRelayCommand AddTriggerCommand => addTriggerCommand ??= new(AddTrigger);

    private async Task AddTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        OptionService.AddTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        Triggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveTriggerCommand => removeTriggerCommand ??= new(RemoveTrigger, CanRemoveTrigger);

    private void RemoveTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        OptionService.RemoveTrigger(Selected.Model, observable.Model);
        Triggers.Remove(observable);
    }
    private bool CanRemoveTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? goToTriggerCommand;
    public RelayCommand<TriggerViewModel> GoToTriggerCommand => goToTriggerCommand ??= new(GoToTrigger, CanGoToTrigger);

    private void GoToTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanGoToTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Trigger Commands

    private RelayCommand? createTriggeredEventCommand;
    public RelayCommand CreateTriggeredEventCommand => createTriggeredEventCommand ??= new(CreateTriggeredEvent);

    private void CreateTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        Event model = new();
        EventService.Insert(model);

        Portrait leftPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.Left,
        };
        Portrait rightPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.Right,
        };
        Portrait lowerLeftPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerLeft,
        };
        Portrait lowerCenterPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerCenter,
        };
        Portrait lowerRightPortrait = new()
        {
            EventId = model.Id,
            Position = PortraitPosition.LowerRight,
        };

        PortraitService.Insert(leftPortrait);
        PortraitService.Insert(rightPortrait);
        PortraitService.Insert(lowerLeftPortrait);
        PortraitService.Insert(lowerCenterPortrait);
        PortraitService.Insert(lowerRightPortrait);

        EventViewModel observable = new() { Model = model };
        AllEvents.Add(observable);

        TriggeredEventId = model.Id;
    }

    private AsyncRelayCommand? addTriggeredEventCommand;
    public AsyncRelayCommand AddTriggeredEventCommand => addTriggeredEventCommand ??= new(AddTriggeredEvent);

    private async Task AddTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = EventService.Get()
            .Select(model => new EventViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        TriggeredEventId = Finder.Selected.Id;
    }

    private RelayCommand? removeTriggeredEventCommand;
    public RelayCommand RemoveTriggeredEventCommand => removeTriggeredEventCommand ??= new(RemoveTriggeredEvent, CanRemoveTriggeredEvent);

    private void RemoveTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (TriggeredEventId == null)
        { return; }

        TriggeredEventId = null;
    }
    private bool CanRemoveTriggeredEvent()
    {
        return TriggeredEventId != null;
    }

    private RelayCommand? editTriggeredEventCommand;
    public RelayCommand EditTriggeredEventCommand => editTriggeredEventCommand ??= new(EditTriggeredEvent, CanEditTriggeredEvent);

    private void EditTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (TriggeredEventId == null)
        { return; }

        Event model = EventService.Get(TriggeredEventId.Value);

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEditTriggeredEvent()
    {
        return TriggeredEventId != null;
    }

    #endregion

    #region Effect Commands

    private RelayCommand? createEffectCommand;
    public RelayCommand CreateEffectCommand => createEffectCommand ??= new(CreateEffect);

    private void CreateEffect()
    {
        if (Selected == null)
        { return; }

        Option owner = Selected.Model;
        Effect relation = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };

        EffectService.Insert(relation);
        OptionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }

    private AsyncRelayCommand? addEffectCommand;
    public AsyncRelayCommand AddEffectCommand => addEffectCommand ??= new(AddEffect);

    private async Task AddEffect()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = EffectService.Get()
            .Select(model => new EffectViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = Selected.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        OptionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeEffectCommand;
    public RelayCommand<EffectViewModel> RemoveEffectCommand => removeEffectCommand ??= new(RemoveEffect, CanRemoveEffect);

    private void RemoveEffect(EffectViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        OptionService.RemoveEffect(Selected.Model, observable.Model);
        Effects.Remove(observable);
    }
    private bool CanRemoveEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? goToEffectCommand;
    public RelayCommand<EffectViewModel> GoToEffectCommand => goToEffectCommand ??= new(GoToEffect, CanGoToEffect);

    private void GoToEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanGoToEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion
}
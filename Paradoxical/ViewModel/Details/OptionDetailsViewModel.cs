using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class OptionDetailsViewModel : DetailsViewModel
    , IEquatable<OptionDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public IDataService DataService { get; }
    public IModService ModService { get; }
    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }

    public IFinder Finder { get; }

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
    public ObservableCollection<TriggerViewModel> Triggers
    {
        get => triggers ??= new();
        set => SetProperty(ref triggers, value);
    }

    private ObservableCollection<EffectViewModel>? effects;
    public ObservableCollection<EffectViewModel> Effects
    {
        get => effects ??= new();
        set => SetProperty(ref effects, value);
    }

    private ObservableCollection<EventViewModel>? allEvents;
    public ObservableCollection<EventViewModel> AllEvents
    {
        get => allEvents ??= new();
        set => SetProperty(ref allEvents, value);
    }

    public string Output
    {
        get
        {
            if (Selected == null)
            { return string.Empty; }

            using StringWriter writer = new();

            Selected.Model.Write(writer, ModService, OptionService);

            return writer.ToString();
        }
    }

    public OptionDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IModService modService,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IEventService eventService,
        IPortraitService portraitService,
        IFinder finder)
        : base(shell, mediator)
    {
        DataService = dataService;
        ModService = modService;
        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
        EventService = eventService;
        PortraitService = portraitService;

        Finder = finder;
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
        model = OptionService.Get(model);
        Selected = new() { Model = model };

        Selected.PropertyChanged += TriggeredEventId_PropertyChanged;

        LoadTriggers();
        LoadEffects();
        LoadAllEvents();

        RefreshOutput();

        DataService.BeginTransaction();
    }

    private void LoadTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = OptionService.GetTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        Triggers = new(triggers);
    }

    private void LoadEffects()
    {
        if (Selected == null)
        { return; }

        var effects = OptionService.GetEffects(Selected.Model)
                    .Select(model => new EffectViewModel() { Model = model });

        Effects = new(effects);
    }

    private void LoadAllEvents()
    {
        if (Selected == null)
        { return; }

        var allEvents = EventService.Get()
            .Select(model => new EventViewModel() { Model = model });

        AllEvents = new(allEvents);
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        var model = Selected.Model;
        Selected = null;

        Load(model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        OptionService.Update(Selected.Model);

        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand? refreshOutputCommand;
    public RelayCommand RefreshOutputCommand => refreshOutputCommand ??= new(RefreshOutput);

    private void RefreshOutput()
    {
        if (Selected == null)
        { return; }

        OnPropertyChanged(nameof(Output));
    }

    #region Flow Commands

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

        Selected.TriggeredEventId = model.Id;
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

        Selected.TriggeredEventId = Finder.Selected.Id;
    }

    private RelayCommand? removeTriggeredEventCommand;
    public RelayCommand RemoveTriggeredEventCommand => removeTriggeredEventCommand ??= new(RemoveTriggeredEvent, CanRemoveTriggeredEvent);

    private void RemoveTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (Selected.TriggeredEventId == null)
        { return; }

        Selected.TriggeredEventId = null;
    }
    private bool CanRemoveTriggeredEvent()
    {
        return Selected != null && Selected.TriggeredEventId != null;
    }

    private RelayCommand? editTriggeredEventCommand;
    public RelayCommand EditTriggeredEventCommand => editTriggeredEventCommand ??= new(EditTriggeredEvent, CanEditTriggeredEvent);

    private void EditTriggeredEvent()
    {
        if (Selected == null)
        { return; }

        if (Selected.TriggeredEventId == null)
        { return; }

        Event model = EventService.Get(Selected.TriggeredEventId.Value);

        var page = Shell.Navigate<EventPageViewModel>();
        //page.Select(model);
    }
    private bool CanEditTriggeredEvent()
    {
        return Selected != null && Selected.TriggeredEventId != null;
    }

    private void TriggeredEventId_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(OptionViewModel.TriggeredEventId))
        { return; }

        RemoveTriggeredEventCommand.NotifyCanExecuteChanged();
        EditTriggeredEventCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Trigger Commands

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger);

    private void CreateTrigger()
    {
        if (Selected == null)
        { return; }

        Option owner = Selected.Model;
        Trigger relation = new();

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

    private RelayCommand<TriggerViewModel>? editTriggerCommand;
    public RelayCommand<TriggerViewModel> EditTriggerCommand => editTriggerCommand ??= new(EditTrigger, CanEditTrigger);

    private void EditTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerPageViewModel>();
        //page.Select(model);
    }
    private bool CanEditTrigger(TriggerViewModel? observable)
    {
        return observable != null;
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
        Effect relation = new();

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

    private RelayCommand<EffectViewModel>? editEffectCommand;
    public RelayCommand<EffectViewModel> EditEffectCommand => editEffectCommand ??= new(EditEffect, CanEditEffect);

    private void EditEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectPageViewModel>();
        //page.Select(model);
    }
    private bool CanEditEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        return Equals(obj as OptionDetailsViewModel);
    }

    public bool Equals(OptionDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<OptionViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(OptionDetailsViewModel? left, OptionDetailsViewModel? right)
    {
        return EqualityComparer<OptionDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(OptionDetailsViewModel? left, OptionDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}
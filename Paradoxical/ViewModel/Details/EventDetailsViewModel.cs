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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : DetailsViewModel
    , IEquatable<EventDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public IDataService DataService { get; }
    public IModService ModService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }
    public IOptionService OptionService { get; }
    public IOnionService OnionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    public IFinder Finder { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private PortraitViewModel? leftPortrait;
    public PortraitViewModel? LeftPortrait
    {
        get => leftPortrait;
        set => SetProperty(ref leftPortrait, value);
    }

    private PortraitViewModel? rightPortrait;
    public PortraitViewModel? RightPortrait
    {
        get => rightPortrait;
        set => SetProperty(ref rightPortrait, value);
    }

    private PortraitViewModel? lowerLeftPortrait;
    public PortraitViewModel? LowerLeftPortrait
    {
        get => lowerLeftPortrait;
        set => SetProperty(ref lowerLeftPortrait, value);
    }

    private PortraitViewModel? lowerCenterPortrait;
    public PortraitViewModel? LowerCenterPortrait
    {
        get => lowerCenterPortrait;
        set => SetProperty(ref lowerCenterPortrait, value);
    }

    private PortraitViewModel? lowerRightPortrait;
    public PortraitViewModel? LowerRightPortrait
    {
        get => lowerRightPortrait;
        set => SetProperty(ref lowerRightPortrait, value);
    }

    private ObservableCollection<OptionViewModel>? options;
    public ObservableCollection<OptionViewModel> Options
    {
        get => options ??= new();
        set
        {
            CommitOptions();

            SetProperty(ref options, value);
        }
    }

    private ICollectionView OptionsView => CollectionViewSource.GetDefaultView(Options);
    private IEditableCollectionView EditableOptionsView => (IEditableCollectionView)OptionsView;

    private void CommitOptions()
    {
        if (EditableOptionsView.IsAddingNew == true)
        {
            EditableOptionsView.CommitNew();
        }
        if (EditableOptionsView.IsEditingItem == true)
        {
            EditableOptionsView.CommitEdit();
        }
    }

    private ObservableCollection<OnionViewModel>? onions;
    public ObservableCollection<OnionViewModel> Onions
    {
        get => onions ??= new();
        set
        {
            CommitOnions();

            SetProperty(ref onions, value);
        }
    }

    private ICollectionView OnionsView => CollectionViewSource.GetDefaultView(Onions);
    private IEditableCollectionView EditableOnionsView => (IEditableCollectionView)OnionsView;

    private void CommitOnions()
    {
        if (EditableOnionsView.IsAddingNew == true)
        {
            EditableOnionsView.CommitNew();
        }
        if (EditableOnionsView.IsEditingItem == true)
        {
            EditableOnionsView.CommitEdit();
        }
    }

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel> Triggers
    {
        get => triggers ??= new();
        set => SetProperty(ref triggers, value);
    }

    private ObservableCollection<EffectViewModel>? immediateEffects;
    public ObservableCollection<EffectViewModel> ImmediateEffects
    {
        get => immediateEffects ??= new();
        set => SetProperty(ref immediateEffects, value);
    }

    private ObservableCollection<EffectViewModel>? afterEffects;
    public ObservableCollection<EffectViewModel> AfterEffects
    {
        get => afterEffects ??= new();
        set => SetProperty(ref afterEffects, value);
    }

    public string Output
    {
        get
        {
            if (Selected == null)
            { return string.Empty; }

            using StringWriter writer = new();

            Selected.Model.Write(writer, ModService, EventService, OptionService, PortraitService);

            return writer.ToString();
        }
    }

    public EventDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IModService modService,
        IEventService eventService,
        IPortraitService portraitService,
        IOptionService optionService,
        IOnionService onionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IFinder finder)
        : base(shell, mediator)
    {
        DataService = dataService;
        ModService = modService;
        EventService = eventService;
        PortraitService = portraitService;
        OptionService = optionService;
        OnionService = onionService;
        TriggerService = triggerService;
        EffectService = effectService;

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

    public void Load(Event model)
    {
        model = EventService.Get(model);
        Selected = new() { Model = model };

        LoadPortraits();
        LoadOptions();
        LoadOnions();
        LoadTriggers();
        LoadImmediateEffects();
        LoadAfterEffects();

        RefreshOutput();

        DataService.BeginTransaction();
    }

    private void LoadPortraits()
    {
        if (Selected == null)
        { return; }

        var leftPortrait = EventService.GetLeftPortrait(Selected.Model);
        var rightPortrait = EventService.GetRightPortrait(Selected.Model);
        var lowerLeftPortrait = EventService.GetLowerLeftPortrait(Selected.Model);
        var lowerCenterPortrait = EventService.GetLowerCenterPortrait(Selected.Model);
        var lowerRightPortrait = EventService.GetLowerRightPortrait(Selected.Model);

        LeftPortrait = new() { Model = leftPortrait };
        RightPortrait = new() { Model = rightPortrait };
        LowerLeftPortrait = new() { Model = lowerLeftPortrait };
        LowerCenterPortrait = new() { Model = lowerCenterPortrait };
        LowerRightPortrait = new() { Model = lowerRightPortrait };
    }

    private void LoadOptions()
    {
        if (Selected == null)
        { return; }

        var options = EventService.GetOptions(Selected.Model)
            .Select(model => new OptionViewModel() { Model = model });

        Options = new(options);
        OptionsView.SortDescriptions.Add(new()
        {
            PropertyName = nameof(OptionViewModel.Priority),
            Direction = ListSortDirection.Ascending,
        });
        OptionsView.SortDescriptions.Add(new()
        {
            PropertyName = nameof(OptionViewModel.Id),
            Direction = ListSortDirection.Ascending,
        });
    }

    private void LoadOnions()
    {
        if (Selected == null)
        { return; }

        var onions = EventService.GetOnions(Selected.Model)
            .Select(model => new OnionViewModel() { Model = model });

        Onions = new(onions);
    }

    private void LoadTriggers()
    {
        if (Selected == null)
        { return; }

        var triggers = EventService.GetTriggers(Selected.Model)
            .Select(model => new TriggerViewModel() { Model = model });

        Triggers = new(triggers);
    }

    private void LoadImmediateEffects()
    {
        if (Selected == null)
        { return; }

        var effects = EventService.GetImmediates(Selected.Model)
            .Select(model => new EffectViewModel() { Model = model });

        ImmediateEffects = new(effects);
    }

    private void LoadAfterEffects()
    {
        if (Selected == null)
        { return; }

        var effects = EventService.GetAfters(Selected.Model)
            .Select(model => new EffectViewModel() { Model = model });

        AfterEffects = new(effects);
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

        CommitOptions();

        SavePortraits();
        SaveOptions();
        SaveOnions();

        EventService.Update(Selected.Model);

        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private void SavePortraits()
    {
        if (LeftPortrait != null)
        {
            PortraitService.Update(LeftPortrait.Model);
        }
        if (RightPortrait != null)
        {
            PortraitService.Update(RightPortrait.Model);
        }
        if (LowerLeftPortrait != null)
        {
            PortraitService.Update(LowerLeftPortrait.Model);
        }
        if (LowerCenterPortrait != null)
        {
            PortraitService.Update(LowerCenterPortrait.Model);
        }
        if (LowerRightPortrait != null)
        {
            PortraitService.Update(LowerRightPortrait.Model);
        }
    }

    private void SaveOptions()
    {
        foreach (var option in Options)
        {
            OptionService.Update(option.Model);
        }
    }

    private void SaveOnions()
    {
        foreach (var onion in Onions)
        {
            OnionService.Update(onion.Model);
        }
    }

    private RelayCommand? refreshOutputCommand;
    public RelayCommand RefreshOutputCommand => refreshOutputCommand ??= new(RefreshOutput);

    private void RefreshOutput()
    {
        if (Selected == null)
        { return; }

        OnPropertyChanged(nameof(Output));
    }

    #region Option Commands

    private RelayCommand? createOptionCommand;
    public RelayCommand CreateOptionCommand => createOptionCommand ??= new(CreateOption);

    private void CreateOption()
    {
        if (Selected == null)
        { return; }

        Option model = new() { EventId = Selected.Id };
        OptionService.Insert(model);

        OptionViewModel observable = new() { Model = model };

        Options.Add(observable);
        OptionsView.MoveCurrentTo(observable);
    }

    private RelayCommand<object>? deleteOptionCommand;
    public RelayCommand<object> DeleteOptionCommand => deleteOptionCommand ??= new(DeleteOption, CanDeleteOption);

    private void DeleteOption(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        CommitOptions();

        Option model = observable.Model;
        OptionService.Delete(model);

        Options.Remove(observable);
    }
    private bool CanDeleteOption(object? param)
    {
        return param is OptionViewModel;
    }

    private RelayCommand<object>? editOptionCommand;
    public RelayCommand<object> EditOptionCommand => editOptionCommand ??= new(EditOption, EditToOption);

    private void EditOption(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        CommitOptions();

        Option model = observable.Model;

        var page = Shell.Navigate<EventPageViewModel>();
        //page.Select(model);
    }
    private bool EditToOption(object? param)
    {
        return param is OptionViewModel;
    }

    #endregion

    #region Onion Commands

    private RelayCommand? createOnionCommand;
    public RelayCommand CreateOnionCommand => createOnionCommand ??= new(CreateOnion);

    private void CreateOnion()
    {
        if (Selected == null)
        { return; }

        Onion model = new() { EventId = Selected.Id };
        OnionService.Insert(model);

        OnionViewModel observable = new() { Model = model };

        Onions.Add(observable);
        OnionsView.MoveCurrentTo(observable);
    }

    private RelayCommand<object>? deleteOnionCommand;
    public RelayCommand<object> DeleteOnionCommand => deleteOnionCommand ??= new(DeleteOnion, CanDeleteOnion);

    private void DeleteOnion(object? param)
    {
        if (param is not OnionViewModel observable)
        { return; }

        CommitOnions();

        Onion model = observable.Model;
        OnionService.Delete(model);

        Onions.Remove(observable);
    }
    private bool CanDeleteOnion(object? param)
    {
        return param is OnionViewModel;
    }

    #endregion

    #region Trigger Commands

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger);

    private void CreateTrigger()
    {
        if (Selected == null)
        { return; }

        Event owner = Selected.Model;
        Trigger relation = new();

        TriggerService.Insert(relation);
        EventService.AddTrigger(owner, relation);

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

        Event owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        EventService.AddTrigger(owner, relation);

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

        EventService.RemoveTrigger(Selected.Model, observable.Model);
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

    #region Immediate Effect Commands

    private RelayCommand? createImmediateEffectCommand;
    public RelayCommand CreateImmediateEffectCommand => createImmediateEffectCommand ??= new(CreateImmediateEffect);

    private void CreateImmediateEffect()
    {
        if (Selected == null)
        { return; }

        Event owner = Selected.Model;
        Effect relation = new();

        EffectService.Insert(relation);
        EventService.AddImmediate(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        ImmediateEffects.Add(observable);
    }

    private AsyncRelayCommand? addImmediateEffectCommand;
    public AsyncRelayCommand AddImmediateEffectCommand => addImmediateEffectCommand ??= new(AddImmediateEffect);

    private async Task AddImmediateEffect()
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

        Event owner = Selected.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.AddImmediate(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        ImmediateEffects.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeImmediateEffectCommand;
    public RelayCommand<EffectViewModel> RemoveImmediateEffectCommand => removeImmediateEffectCommand ??= new(RemoveImmediateEffect, CanRemoveImmediateEffect);

    private void RemoveImmediateEffect(EffectViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        EventService.RemoveImmediate(Selected.Model, observable.Model);
        ImmediateEffects.Remove(observable);
    }
    private bool CanRemoveImmediateEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? editImmediateEffectCommand;
    public RelayCommand<EffectViewModel> EditImmediateEffectCommand => editImmediateEffectCommand ??= new(EditImmediateEffect, CanEditImmediateEffect);

    private void EditImmediateEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectPageViewModel>();
        //page.Select(model);
    }
    private bool CanEditImmediateEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region After Effect Commands

    private RelayCommand? createAfterEffectCommand;
    public RelayCommand CreateAfterEffectCommand => createAfterEffectCommand ??= new(CreateAfterEffect);

    private void CreateAfterEffect()
    {
        if (Selected == null)
        { return; }

        Event owner = Selected.Model;
        Effect relation = new();

        EffectService.Insert(relation);
        EventService.AddAfter(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        AfterEffects.Add(observable);
    }

    private AsyncRelayCommand? addAfterEffectCommand;
    public AsyncRelayCommand AddAfterEffectCommand => addAfterEffectCommand ??= new(AddAfterEffect);

    private async Task AddAfterEffect()
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

        Event owner = Selected.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.AddAfter(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        AfterEffects.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeAfterEffectCommand;
    public RelayCommand<EffectViewModel> RemoveAfterEffectCommand => removeAfterEffectCommand ??= new(RemoveAfterEffect, CanRemoveAfterEffect);

    private void RemoveAfterEffect(EffectViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        EventService.RemoveAfter(Selected.Model, observable.Model);
        AfterEffects.Remove(observable);
    }
    private bool CanRemoveAfterEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? editAfterEffectCommand;
    public RelayCommand<EffectViewModel> EditAfterEffectCommand => editAfterEffectCommand ??= new(EditAfterEffect, CanEditAfterEffect);

    private void EditAfterEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectPageViewModel>();
        //page.Select(model);
    }
    private bool CanEditAfterEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        return Equals(obj as EventDetailsViewModel);
    }

    public bool Equals(EventDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<EventViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(EventDetailsViewModel? left, EventDetailsViewModel? right)
    {
        return EqualityComparer<EventDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(EventDetailsViewModel? left, EventDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}

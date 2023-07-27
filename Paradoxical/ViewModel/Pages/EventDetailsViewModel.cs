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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModel
    , IEquatable<EventDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Event Details";

    public override bool IsValid
    {
        get
        {
            if (Selected == null)
            { return false; }

            var model = EventService.Find(Selected.Model);

            if (model == null)
            { return false; }

            return true;
        }
    }

    public IFinder Finder { get; }

    public IModService ModService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }
    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

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
        get
        {
            if (options == null)
            {
                options = new();
                options.CollectionChanged += Options_CollectionChanged;
            }

            return options;
        }
        set
        {
            OnPropertyChanging();

            options = value;
            options.CollectionChanged += Options_CollectionChanged;

            OnPropertyChanged();
        }
    }

    private void Options_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count == 1)
        {
            OptionViewModel observable = e.NewItems.Cast<OptionViewModel>().Single();
            OptionService.Insert(observable.Model);

            observable.Model.EventId = Selected?.Id ?? 0;
        }
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count == 1)
        {
            OptionViewModel observable = e.OldItems.Cast<OptionViewModel>().Single();
            OptionService.Delete(observable.Model);

            Shell.ValidatePages();
        }
    }

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel> Triggers => triggers ??= new();

    private ObservableCollection<EffectViewModel>? immediateEffects;
    public ObservableCollection<EffectViewModel> ImmediateEffects => immediateEffects ??= new();

    private ObservableCollection<EffectViewModel>? afterEffects;
    public ObservableCollection<EffectViewModel> AfterEffects => afterEffects ??= new();

    public EventDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IFinder finder,
        IModService modService,
        IEventService eventService,
        IPortraitService portraitService,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService)
        : base(shell, mediator)
    {
        Finder = finder;

        ModService = modService;
        EventService = eventService;
        PortraitService = portraitService;
        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
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

    public void Load(Event model)
    {
        var selected = EventService.Get(model);

        var leftPortrait = EventService.GetLeftPortrait(model);
        var rightPortrait = EventService.GetRightPortrait(model);
        var lowerLeftPortrait = EventService.GetLowerLeftPortrait(model);
        var lowerCenterPortrait = EventService.GetLowerCenterPortrait(model);
        var lowerRightPortrait = EventService.GetLowerRightPortrait(model);

        var options = EventService.GetOptions(selected)
            .Select(model => new OptionViewModel() { Model = model });

        var triggers = EventService.GetTriggers(selected)
            .Select(model => new TriggerViewModel() { Model = model });

        var immediates = EventService.GetImmediates(selected)
            .Select(model => new EffectViewModel() { Model = model });

        var afters = EventService.GetAfters(selected)
            .Select(model => new EffectViewModel() { Model = model });

        Selected = new() { Model = selected };

        LeftPortrait = new() { Model = leftPortrait };
        RightPortrait = new() { Model = rightPortrait };
        LowerLeftPortrait = new() { Model = lowerLeftPortrait };
        LowerCenterPortrait = new() { Model = lowerCenterPortrait };
        LowerRightPortrait = new() { Model = lowerRightPortrait };

        Options = new(options);

        Triggers.Clear();
        Triggers.AddRange(triggers);

        ImmediateEffects.Clear();
        ImmediateEffects.AddRange(immediates);

        AfterEffects.Clear();
        AfterEffects.AddRange(afters);

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

        EventService.Update(Selected.Model);

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

        foreach (var option in Options)
        {
            OptionService.Update(option.Model);
        }
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

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Event model = new(Selected.Model);
        EventService.Insert(model);

        var options = EventService.GetOptions(Selected.Model);
        foreach (var option in options)
        {
            Option duplicate = new(option);
            duplicate.EventId = model.Id;
            OptionService.Insert(duplicate);
        }

        var portraits = EventService.GetPortraits(Selected.Model);
        foreach (var portrait in portraits)
        {
            Portrait duplicate = new(portrait);
            duplicate.EventId = model.Id;
            PortraitService.Insert(duplicate);
        }

        var triggers = EventService.GetTriggers(Selected.Model);
        foreach (var trigger in triggers)
        {
            EventService.AddTrigger(model, trigger);
        }

        var immediates = EventService.GetImmediates(Selected.Model);
        foreach (var immediate in immediates)
        {
            EventService.AddImmediate(model, immediate);
        }

        var afters = EventService.GetAfters(Selected.Model);
        foreach (var after in afters)
        {
            EventService.AddAfter(model, after);
        }

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        EventService.Delete(Selected.Model);

        Shell.Navigate<EventTableViewModel>();
        Shell.InvalidatePage(this);
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

        Selected.Model.Write(writer, ModService, EventService, OptionService, PortraitService);

        return writer.ToString();
    }

    #endregion

    #region Option Commands

    private RelayCommand? createOptionCommand;
    public RelayCommand CreateOptionCommand => createOptionCommand ??= new(CreateOption);

    private void CreateOption()
    {
        if (Selected == null)
        { return; }

        OptionViewModel item = new();

        Options.Add(item);
        CollectionViewSource.GetDefaultView(Options).MoveCurrentTo(item);
    }

    private RelayCommand<object>? deleteOptionCommand;
    public RelayCommand<object> DeleteOptionCommand => deleteOptionCommand ??= new(DeleteOption, CanDeleteOption);

    private void DeleteOption(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

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

        Option model = observable.Model;

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }
    private bool EditToOption(object? param)
    {
        return param is OptionViewModel;
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
        Trigger relation = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

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

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
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
        Effect relation = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };

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

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
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
        Effect relation = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };

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

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
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

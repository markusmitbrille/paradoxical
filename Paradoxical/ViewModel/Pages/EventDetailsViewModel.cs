using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Event Details";

    public FinderViewModel Finder { get; }

    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }
    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

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
    public ObservableCollection<OptionViewModel> Options => options ??= new();

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel> Triggers => triggers ??= new();

    private ObservableCollection<EffectViewModel>? immediates;
    public ObservableCollection<EffectViewModel> Immediates => immediates ??= new();

    private ObservableCollection<EffectViewModel>? afters;
    public ObservableCollection<EffectViewModel> Afters => afters ??= new();

    public EventDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        FinderViewModel finder,
        IEventService eventService,
        IPortraitService portraitService,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService)
        : base(shell, mediator)
    {
        Finder = finder;

        EventService = eventService;
        PortraitService = portraitService;
        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    protected override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatingFrom()
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

        Options.Clear();
        Options.AddRange(options);

        Triggers.Clear();
        Triggers.AddRange(triggers);

        Immediates.Clear();
        Immediates.AddRange(immediates);

        Afters.Clear();
        Afters.AddRange(afters);
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload, CanReload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }
    private bool CanReload()
    {
        return Selected != null;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save, CanSave);

    private void Save()
    {
        if (Selected == null)
        { return; }

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
    }
    public bool CanSave()
    {
        return Selected != null;
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Event model = new()
        {
            Name = $"evt_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Title = "Hello World",
            Description = "Hello World!",
        };

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
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Event model = new(Selected.Model);

        EventService.Insert(model);

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }
    private bool CanDuplicate()
    {
        return Selected != null;
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        EventService.Delete(Selected.Model);

        Shell.Navigate<EventTableViewModel>();

        var historyPages = Shell.PageHistory.OfType<EventDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<EventDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }
    private bool CanDelete()
    {
        return Selected != null;
    }

    #region Option Commands

    private RelayCommand? createOptionCommand;
    public RelayCommand CreateOptionCommand => createOptionCommand ??= new(CreateOption);

    private void CreateOption()
    {
        if (Selected == null)
        { return; }

        Option model = new() { EventId = Selected.Id };
        OptionViewModel observable = new() { Model = model };

        OptionService.Insert(model);
        Options.Add(observable);
    }

    private RelayCommand<OptionViewModel>? removeOptionCommand;
    public RelayCommand<OptionViewModel> RemoveOptionCommand => removeOptionCommand ??= new(RemoveOption, CanRemoveOption);

    private void RemoveOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return; }

        Option model = observable.Model;

        OptionService.Delete(model);
        Options.Remove(observable);
    }
    private bool CanRemoveOption(OptionViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<OptionViewModel>? findOptionCommand;
    public RelayCommand<OptionViewModel> FindOptionCommand => findOptionCommand ??= new(FindOption, CanFindOption);

    private void FindOption(OptionViewModel? observable)
    {
        if (observable == null)
        { return; }

        Option model = observable.Model;

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanFindOption(OptionViewModel? observable)
    {
        return observable != null;
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
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        TriggerService.Delete(model);
        Triggers.Remove(observable);
    }
    private bool CanRemoveTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? findTriggerCommand;
    public RelayCommand<TriggerViewModel> FindTriggerCommand => findTriggerCommand ??= new(FindTrigger, CanFindTrigger);

    private void FindTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanFindTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Immediate Commands

    private RelayCommand? createImmediateCommand;
    public RelayCommand CreateImmediateCommand => createImmediateCommand ??= new(CreateImmediate);

    private void CreateImmediate()
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
        Immediates.Add(observable);
    }

    private AsyncRelayCommand? addImmediateCommand;
    public AsyncRelayCommand AddImmediateCommand => addImmediateCommand ??= new(AddImmediate);

    private async Task AddImmediate()
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
        Immediates.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeImmediateCommand;
    public RelayCommand<EffectViewModel> RemoveImmediateCommand => removeImmediateCommand ??= new(RemoveImmediate, CanRemoveImmediate);

    private void RemoveImmediate(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        EffectService.Delete(model);
        Immediates.Remove(observable);
    }
    private bool CanRemoveImmediate(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? findImmediateCommand;
    public RelayCommand<EffectViewModel> FindImmediateCommand => findImmediateCommand ??= new(FindImmediate, CanFindImmediate);

    private void FindImmediate(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanFindImmediate(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region After Commands

    private RelayCommand? createAfterCommand;
    public RelayCommand CreateAfterCommand => createAfterCommand ??= new(CreateAfter);

    private void CreateAfter()
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
        Afters.Add(observable);
    }

    private AsyncRelayCommand? addAfterCommand;
    public AsyncRelayCommand AddAfterCommand => addAfterCommand ??= new(AddAfter);

    private async Task AddAfter()
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
        Afters.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeAfterCommand;
    public RelayCommand<EffectViewModel> RemoveAfterCommand => removeAfterCommand ??= new(RemoveAfter, CanRemoveAfter);

    private void RemoveAfter(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        EffectService.Delete(model);
        Afters.Remove(observable);
    }
    private bool CanRemoveAfter(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? findAfterCommand;
    public RelayCommand<EffectViewModel> FindAfterCommand => findAfterCommand ??= new(FindAfter, CanFindAfter);

    private void FindAfter(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanFindAfter(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion
}

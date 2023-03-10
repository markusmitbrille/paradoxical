using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model;
using Paradoxical.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModelBase,
    IElementDetailsViewModel,
    IMessageHandler<ElementSelectedMessage>,
    IMessageHandler<RelationAddedMessage>,
    IMessageHandler<RelationRemovedMessage>,
    IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Event Details";

    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public IEventService EventService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    IElementViewModel? IElementDetailsViewModel.Selected => Selected;

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel>? Triggers
    {
        get => triggers;
        private set => SetProperty(ref triggers, value);
    }

    private ObservableCollection<EffectViewModel>? immediateEffects;
    public ObservableCollection<EffectViewModel>? ImmediateEffects
    {
        get => immediateEffects;
        private set => SetProperty(ref immediateEffects, value);
    }

    private ObservableCollection<EffectViewModel>? afterEffects;
    public ObservableCollection<EffectViewModel>? AfterEffects
    {
        get => afterEffects;
        private set => SetProperty(ref afterEffects, value);
    }

    public EventDetailsViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        IEventService eventService,
        ITriggerService triggerService,
        IEffectService effectService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        EventService = eventService;
        TriggerService = triggerService;
        EffectService = effectService;

        Mediator.Register<ElementSelectedMessage>(this);
        Mediator.Register<RelationAddedMessage>(this);
        Mediator.Register<RelationRemovedMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatedTo()
    {
        base.OnNavigatedTo();

        Load();
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();
    }

    public void Handle(ElementSelectedMessage message)
    {
        if (message.Element is not Event model)
        { return; }

        if (Navigation.CurrentPage != this)
        { return; }

        var selected = EventService.Get(model);

        var triggers = EventService.GetTriggers(selected)
            .Select(trigger => new TriggerViewModel(trigger));
        var immediateEffects = EventService.GetImmediateEffects(selected)
            .Select(effect => new EffectViewModel(effect));
        var afterEffects = EventService.GetAfterEffects(selected)
            .Select(effect => new EffectViewModel(effect));

        Selected = new(selected);

        Triggers = new(triggers);
        ImmediateEffects = new(immediateEffects);
        AfterEffects = new(afterEffects);
    }

    public void Handle(RelationAddedMessage message)
    {
        HandleEventTrigger(message);
        HandleEventImmediateEffect(message);
        HandleEventAfterEffect(message);

        void HandleEventTrigger(RelationAddedMessage message)
        {
            if (message.Relation is EventTrigger relation)
            {
                var model = TriggerService.Get(relation.TriggerId);
                Triggers?.Add(new(model));
            }
        }
        void HandleEventImmediateEffect(RelationAddedMessage message)
        {
            if (message.Relation is EventImmediateEffect relation)
            {
                var model = EffectService.Get(relation.EffectId);
                ImmediateEffects?.Add(new(model));
            }
        }
        void HandleEventAfterEffect(RelationAddedMessage message)
        {
            if (message.Relation is EventAfterEffect relation)
            {
                var model = EffectService.Get(relation.EffectId);
                AfterEffects?.Add(new(model));
            }
        }
    }

    public void Handle(RelationRemovedMessage message)
    {
        HandleEventTrigger(message);
        HandleEventImmediateEffect(message);
        HandleEventAfterEffect(message);

        void HandleEventTrigger(RelationRemovedMessage message)
        {
            if (message.Relation is EventTrigger relation)
            {
                var model = TriggerService.Get(relation.TriggerId);
                Triggers?.Remove(new(model));
            }
        }
        void HandleEventImmediateEffect(RelationRemovedMessage message)
        {
            if (message.Relation is EventImmediateEffect relation)
            {
                var model = EffectService.Get(relation.EffectId);
                ImmediateEffects?.Remove(new(model));
            }
        }
        void HandleEventAfterEffect(RelationRemovedMessage message)
        {
            if (message.Relation is EventAfterEffect relation)
            {
                var model = EffectService.Get(relation.EffectId);
                AfterEffects?.Remove(new(model));
            }
        }
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        if (Selected == null)
        { return; }

        var selected = EventService.Get(Selected.Model);

        var triggers = EventService.GetTriggers(selected)
            .Select(trigger => new TriggerViewModel(trigger));
        var immediateEffects = EventService.GetImmediateEffects(selected)
            .Select(effect => new EffectViewModel(effect));
        var afterEffects = EventService.GetAfterEffects(selected)
            .Select(effect => new EffectViewModel(effect));

        Selected = new(selected);

        Triggers = new(triggers);
        ImmediateEffects = new(immediateEffects);
        AfterEffects = new(afterEffects);
    }

    private void Save()
    {
        if (Selected == null)
        { return; }

        EventService.Update(Selected.Model);
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        Save();

        Finder.Items = EventService.Get().Select(model => new EventViewModel(model));
        Finder.Selected = Selected;

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Navigation.Navigate<EventDetailsViewModel>();
        Mediator.Send<ElementSelectedMessage>(new(Finder.Selected.Model));
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        if (Selected != null)
        {
            Navigation.Navigate<EventDetailsViewModel>();
        }

        Event model = new()
        {
            Name = $"evt_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Title = "Hello World",
            Description = "Hello World!",
        };

        EventService.Insert(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Navigation.Navigate<EventDetailsViewModel>();

        Event model = new(Selected.Model);
        EventService.Insert(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        EventService.Delete(Selected.Model);
    }

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger);

    private void CreateTrigger()
    {
        if (Selected == null)
        { return; }

        if (Triggers == null)
        { return; }

        Trigger trigger = new()
        {
            Name = $"trg_{Selected.Name}_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = $"# {Selected.Name} Trigger",
        };

        TriggerService.Insert(trigger);

        EventService.AddTrigger(new()
        {
            TriggerId = trigger.Id,
            EventId = Selected.Id
        });
    }

    private AsyncRelayCommand? addTriggerCommand;
    public AsyncRelayCommand AddTriggerCommand => addTriggerCommand ??= new(AddTrigger);

    private async Task AddTrigger()
    {
        if (Selected == null)
        { return; }

        if (Triggers == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel(model))
            .Except(Triggers);

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        if (Finder.Selected is not TriggerViewModel trigger)
        { return; }

        EventService.AddTrigger(new()
        {
            TriggerId = trigger.Id,
            EventId = Selected.Id
        });
    }

    private RelayCommand<TriggerViewModel>? removeTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveTriggerCommand => removeTriggerCommand ??= new(RemoveTrigger, CanRemoveTrigger);

    private void RemoveTrigger(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        if (Selected == null)
        { return; }

        if (Triggers == null)
        { return; }

        EventService.RemoveTrigger(new()
        {
            TriggerId = viewmodel.Model.Id,
            EventId = Selected.Id
        });
    }
    private bool CanRemoveTrigger(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<TriggerViewModel>? findTriggerCommand;
    public RelayCommand<TriggerViewModel> FindTriggerCommand => findTriggerCommand ??= new(FindTrigger, CanFindTrigger);

    private void FindTrigger(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<ElementSelectedMessage>(new(viewmodel.Model));
    }
    private bool CanFindTrigger(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }
}

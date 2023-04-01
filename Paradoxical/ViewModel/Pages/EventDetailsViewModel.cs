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
    , IMessageHandler<SelectMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Event Details";

    public FinderViewModel Finder { get; }

    public IMediatorService Mediator { get; }

    public IEventService EventService { get; }
    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
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
        NavigationViewModel navigation,
        FinderViewModel finder,
        IMediatorService mediator,
        IEventService eventService,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService)
        : base(navigation)
    {
        Finder = finder;

        Mediator = mediator;

        EventService = eventService;
        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    protected override void OnNavigatedTo()
    {
        base.OnNavigatedTo();

        Load();

        Mediator.Register<SelectMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();

        Mediator.Unregister<SelectMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    public void Handle(SelectMessage message)
    {
        if (message.Model is not Event model)
        { return; }

        var selected = EventService.Get(model);
        Selected = new() { Model = selected };
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

        var options = EventService.GetOptions(selected)
            .Select(model => new OptionViewModel() { Model = model });

        var triggers = EventService.GetTriggers(selected)
            .Select(model => new TriggerViewModel() { Model = model });

        var immediates = EventService.GetImmediates(selected)
            .Select(model => new EffectViewModel() { Model = model });

        var afters = EventService.GetAfters(selected)
            .Select(model => new EffectViewModel() { Model = model });

        Selected = new() { Model = selected };

        Options.Clear();
        Options.AddRange(options);

        Triggers.Clear();
        Triggers.AddRange(triggers);

        Immediates.Clear();
        Immediates.AddRange(immediates);

        Afters.Clear();
        Afters.AddRange(afters);
    }

    private void Save()
    {
        if (Selected == null)
        { return; }

        EventService.Update(Selected.Model);
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
        Mediator.Send<SelectMessage>(new(model));
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
        Mediator.Send<SelectMessage>(new(model));
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        EventService.Delete(Selected.Model);
    }

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

        Navigation.Navigate<OptionDetailsViewModel>();
        Mediator.Send<SelectMessage>(new(model));
    }
    private bool CanFindOption(OptionViewModel? observable)
    {
        return observable != null;
    }

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

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<SelectMessage>(new(model));
    }
    private bool CanFindTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }
}

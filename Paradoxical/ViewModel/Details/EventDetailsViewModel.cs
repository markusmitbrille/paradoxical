using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class EventDetailsViewModel : PageViewModelBase,
    IElementDetailsViewModel,
    IMessageHandler<ElementSelectedMessage>,
    IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Event Details";

    public FindDialogViewModel Finder { get; }

    public EventOption Options { get; }
    public EventTriggerViewModel Triggers { get; }
    public EventImmediateViewModel Immediates { get; }
    public EventAfterViewModel Afters { get; }

    public IMediatorService Mediator { get; }

    public IEventService EventService { get; }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    IElementViewModel? IElementDetailsViewModel.Selected => Selected;

    public EventDetailsViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        EventOption options,
        EventTriggerViewModel triggers,
        EventImmediateViewModel immediates,
        EventAfterViewModel afters,
        IMediatorService mediator,
        IEventService eventService)
        : base(navigation)
    {
        Finder = finder;

        Options = options;
        Triggers = triggers;
        Immediates = immediates;
        Afters = afters;

        Mediator = mediator;

        EventService = eventService;

        Mediator.Register<ElementSelectedMessage>(this);
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
        Selected = new(selected);

        Triggers.Fetch(selected);
        Immediates.Fetch(selected);
        Afters.Fetch(selected);
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
        Selected = new(selected);

        Triggers.Fetch(selected);
        Immediates.Fetch(selected);
        Afters.Fetch(selected);
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
}

using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model;
using Paradoxical.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class EventTableViewModel : PageViewModelBase,
    IElementTableViewModel,
    IMessageHandler<ElementSelectedMessage>,
    IMessageHandler<ElementDeselectedMessage>,
    IMessageHandler<ElementInsertedMessage>,
    IMessageHandler<ElementDeletedMessage>,
    IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Events";

    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public IEventService EventService { get; }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    IElementViewModel? IElementTableViewModel.Selected => Selected;

    private string? filter;
    public string? Filter
    {
        get => filter;
        set => SetProperty(ref filter, value);
    }

    public EventTableViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        IEventService triggerService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        EventService = triggerService;

        Mediator.Register<ElementSelectedMessage>(this);
        Mediator.Register<ElementDeselectedMessage>(this);
        Mediator.Register<ElementInsertedMessage>(this);
        Mediator.Register<ElementDeletedMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Events))
        {
            UpdateView();
            UpdateSelected();
        }
        if (e.PropertyName == nameof(Filter))
        {
            UpdateView();
            UpdateSelected();
        }
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

        Selected = Events.SingleOrDefault(viewmodel => viewmodel.Model == model);
    }

    public void Handle(ElementDeselectedMessage message)
    {
        if (message.Element is not Event model)
        { return; }

        if (Navigation.CurrentPage != this)
        { return; }

        if (Selected?.Model == model)
        {
            Selected = null;
        }
    }

    public void Handle(ElementInsertedMessage message)
    {
        if (message.Model is not Event model)
        { return; }

        Events.Add(new(model));
    }

    public void Handle(ElementDeletedMessage message)
    {
        if (message.Model is not Event model)
        { return; }

        var viewmodels = Events.Where(viewmodel => viewmodel.Model == model).ToArray();
        foreach (var viewmodel in viewmodels)
        {
            Events.Remove(viewmodel);
        }
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        Events = new(EventService.Get().Select(model => new EventViewModel(model)));

        ICollectionView view = CollectionViewSource.GetDefaultView(Events);
        view.Filter = Predicate;
    }

    private void Save()
    {
        EventService.UpdateAll(Events.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not EventViewModel viewmodel)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        if (ParadoxPattern.IdFilterRegex.FuzzyMatch(viewmodel.Id.ToString(), Filter) == true)
        { return true; }

        if (ParadoxPattern.NameFilterRegex.FuzzyMatch(viewmodel.Name, Filter) == true)
        { return true; }

        if (ParadoxPattern.TitleFilterRegex.FuzzyMatch(viewmodel.Title, Filter) == true)
        { return true; }

        if (ParadoxPattern.DescriptionFilterRegex.FuzzyMatch(viewmodel.Description, Filter) == true)
        { return true; }

        if (ParadoxPattern.FilterRegex.FuzzyMatch(viewmodel.Name, Filter) == true)
        { return true; }

        return false;
    }

    private void UpdateView()
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(Events);
        view.Refresh();
    }

    private void UpdateSelected()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        ICollectionView view = CollectionViewSource.GetDefaultView(Events);
        Selected = view.Cast<EventViewModel>().FirstOrDefault();
    }

    private ObservableCollection<EventViewModel> triggers = new();
    public ObservableCollection<EventViewModel> Events
    {
        get => triggers;
        set => SetProperty(ref triggers, value);
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        Save();

        Finder.Items = EventService.Get().Select(model => new EventViewModel(model));
        Finder.Selected = Selected;
        Finder.NameFilter = Filter;

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
        Event model = new()
        {
            Name = $"evt_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Title = "Hello World",
            Description = "Hello World!",
        };

        EventService.Insert(model);
    }

    private RelayCommand<EventViewModel>? duplicateCommand;
    public RelayCommand<EventViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(EventViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Event model = new(viewmodel.Model);
        EventService.Insert(model);
    }
    private bool CanDuplicate(EventViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<EventViewModel>? deleteCommand;
    public RelayCommand<EventViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EventViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        EventService.Delete(viewmodel.Model);
    }
    private bool CanDelete(EventViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<EventViewModel>? editCommand;
    public RelayCommand<EventViewModel> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(EventViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Navigation.Navigate<EventDetailsViewModel>();
        Mediator.Send<ElementSelectedMessage>(new(viewmodel.Model));
    }
    private bool CanEdit(EventViewModel? viewmodel)
    {
        return viewmodel != null;
    }
}

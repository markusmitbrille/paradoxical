using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class EventTableViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Events";

    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }

    private ObservableCollection<EventViewModel> items = new();
    public ObservableCollection<EventViewModel> Items
    {
        get => items;
        set => SetProperty(ref items, value);
    }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private string? filter;
    public string? Filter
    {
        get => filter;
        set => SetProperty(ref filter, value);
    }

    public EventTableViewModel(
        IShell shell,
        IMediatorService mediator,
        IEventService eventService,
        IPortraitService portraitService)
        : base(shell, mediator)
    {
        EventService = eventService;
        PortraitService = portraitService;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Items))
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
        Load();

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

    private RelayCommand? loadCommand;
    public RelayCommand LoadCommand => loadCommand ??= new(Load);

    private void Load()
    {
        Items = new(EventService.Get().Select(model => new EventViewModel() { Model = model }));

        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        view.Filter = Predicate;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        EventService.UpdateAll(Items.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not EventViewModel wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        // feature filters

        bool?[] features = new[]
        {
            ParadoxPattern.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter),
            ParadoxPattern.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter),
            ParadoxPattern.TitleFilterRegex.FuzzyMatch(wrapper.Title, Filter),
            ParadoxPattern.DescriptionFilterRegex.FuzzyMatch(wrapper.Description, Filter),
        };

        if (features.Any(res => res == true) && features.All(res => res != false))
        { return true; }

        // general filter

        if (ParadoxPattern.FilterRegex.FuzzyMatch(wrapper.Name, Filter) == true)
        { return true; }

        return false;
    }

    private void UpdateView()
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        view.Refresh();
    }

    private void UpdateSelected()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        Selected = view.Cast<EventViewModel>().FirstOrDefault();
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

        EventViewModel observable = new() { Model = model };
        Items.Add(observable);
    }

    private RelayCommand<EventViewModel>? duplicateCommand;
    public RelayCommand<EventViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(EventViewModel? observable)
    {
        if (observable == null)
        { return; }

        Event model = new(observable.Model);
        EventService.Insert(model);
    }
    private bool CanDuplicate(EventViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EventViewModel>? deleteCommand;
    public RelayCommand<EventViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EventViewModel? observable)
    {
        if (observable == null)
        { return; }

        EventService.Delete(observable.Model);

        var historyPages = Shell.PageHistory.OfType<EventDetailsViewModel>()
            .Where(page => page.Selected?.Model == observable.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<EventDetailsViewModel>()
            .Where(page => page.Selected?.Model == observable.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));

        Items.Remove(observable);
    }
    private bool CanDelete(EventViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EventViewModel>? editCommand;
    public RelayCommand<EventViewModel> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(EventViewModel? observable)
    {
        if (observable == null)
        { return; }

        var model = observable.Model;
       
        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEdit(EventViewModel? observable)
    {
        return observable != null;
    }
}

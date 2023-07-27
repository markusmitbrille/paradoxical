using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public IOptionService OptionService { get; }
    public IPortraitService PortraitService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ObservableCollection<EventViewModel> items = new();
    public ObservableCollection<EventViewModel> Items
    {
        get
        {
            if (items == null)
            {
                items = new();
                items.CollectionChanged += Items_CollectionChanged;
            }

            return items;
        }
        set
        {
            OnPropertyChanging();

            items = value;
            items.CollectionChanged += Items_CollectionChanged;

            UpdateView();
            UpdateSelected();

            OnPropertyChanged();
        }
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count == 1)
        {
            EventViewModel observable = e.NewItems.Cast<EventViewModel>().Single();
            EventService.Insert(observable.Model);

            Portrait leftPortrait = new()
            {
                EventId = observable.Model.Id,
                Position = PortraitPosition.Left,
            };
            Portrait rightPortrait = new()
            {
                EventId = observable.Model.Id,
                Position = PortraitPosition.Right,
            };
            Portrait lowerLeftPortrait = new()
            {
                EventId = observable.Model.Id,
                Position = PortraitPosition.LowerLeft,
            };
            Portrait lowerCenterPortrait = new()
            {
                EventId = observable.Model.Id,
                Position = PortraitPosition.LowerCenter,
            };
            Portrait lowerRightPortrait = new()
            {
                EventId = observable.Model.Id,
                Position = PortraitPosition.LowerRight,
            };

            PortraitService.Insert(leftPortrait);
            PortraitService.Insert(rightPortrait);
            PortraitService.Insert(lowerLeftPortrait);
            PortraitService.Insert(lowerCenterPortrait);
            PortraitService.Insert(lowerRightPortrait);
        }
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count == 1)
        {
            EventViewModel observable = e.OldItems.Cast<EventViewModel>().Single();
            EventService.Delete(observable.Model);

            Shell.ValidatePages();
        }
    }

    private string? filter;
    public string? Filter
    {
        get => filter;
        set
        {
            OnPropertyChanging();

            filter = value;

            UpdateView();
            UpdateSelected();

            OnPropertyChanged();
        }
    }

    private EventViewModel? selected;
    public EventViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public EventTableViewModel(
        IShell shell,
        IMediatorService mediator,
        IEventService eventService,
        IOptionService optionService,
        IPortraitService portraitService)
        : base(shell, mediator)
    {
        EventService = eventService;
        OptionService = optionService;
        PortraitService = portraitService;
    }

    public override void OnNavigatedTo()
    {
        Load();

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

    private RelayCommand? loadCommand;
    public RelayCommand LoadCommand => loadCommand ??= new(Load);

    private void Load()
    {
        Selected = null;
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
        Selected = view.OfType<EventViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Items.Add(new());
    }

    private RelayCommand<EventViewModel>? deleteCommand;
    public RelayCommand<EventViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EventViewModel? observable)
    {
        if (observable == null)
        { return; }

        Items.Remove(observable);
    }
    private bool CanDelete(EventViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<object>? editCommand;
    public RelayCommand<object> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(object? param)
    {
        if (param == null)
        { return; }

        if (param is not EventViewModel observable)
        { return; }

        var model = observable.Model;

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEdit(object? param)
    {
        return param is EventViewModel observable && observable != null;
    }
}

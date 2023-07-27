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

public class TriggerTableViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Triggers";

    public ITriggerService TriggerService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ObservableCollection<TriggerViewModel> items = new();
    public ObservableCollection<TriggerViewModel> Items
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
            TriggerViewModel observable = e.NewItems.Cast<TriggerViewModel>().Single();
            TriggerService.Insert(observable.Model);
        }
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count == 1)
        {
            TriggerViewModel observable = e.OldItems.Cast<TriggerViewModel>().Single();
            TriggerService.Delete(observable.Model);

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

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public TriggerTableViewModel(
        IShell shell,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(shell, mediator)
    {
        TriggerService = triggerService;
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
        Items = new(TriggerService.Get().Select(model => new TriggerViewModel() { Model = model }));

        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        view.Filter = Predicate;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        TriggerService.UpdateAll(Items.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not TriggerViewModel wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        // feature filters

        bool?[] features = new[]
        {
            ParadoxPattern.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter),
            ParadoxPattern.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter),
            ParadoxPattern.CodeFilterRegex.FuzzyMatch(wrapper.Code, Filter),
            ParadoxPattern.TooltipFilterRegex.FuzzyMatch(wrapper.Tooltip, Filter),
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
        Selected = view.OfType<TriggerViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        TriggerViewModel item = new();

        Items.Add(item);
        Selected = item;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Items.Remove(observable);
    }
    private bool CanDelete(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<object>? editCommand;
    public RelayCommand<object> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(object? param)
    {
        if (param == null)
        { return; }

        if (param is not TriggerViewModel observable)
        { return; }

        var model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEdit(object? param)
    {
        return param is TriggerViewModel observable && observable != null;
    }
}

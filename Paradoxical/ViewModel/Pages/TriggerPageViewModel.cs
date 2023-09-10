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

public class TriggerPageViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Triggers";

    public IDataService DataService { get; }
    public ITriggerService TriggerService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ObservableCollection<TriggerViewModel>? items;
    public ObservableCollection<TriggerViewModel> Items
    {
        get => items ??= new();
        set
        {
            CommitItems();

            SetProperty(ref items, value);

            UpdateView();
            UpdateSelected();
        }
    }

    private ICollectionView ItemsView => CollectionViewSource.GetDefaultView(Items);
    private IEditableCollectionView EditableItemsView => (IEditableCollectionView)ItemsView;

    private void CommitItems()
    {
        if (EditableItemsView.IsAddingNew == true)
        {
            EditableItemsView.CommitNew();
        }
        if (EditableItemsView.IsEditingItem == true)
        {
            EditableItemsView.CommitEdit();
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

    public TriggerPageViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        ITriggerService triggerService)
        : base(shell, mediator)
    {
        DataService = dataService;
        TriggerService = triggerService;
    }

    public override void OnNavigatedTo()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Load();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

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
        ItemsView.Filter = Predicate;

        DataService.BeginTransaction();
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Load();
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        CommitItems();

        TriggerService.UpdateAll(Items.Select(vm => vm.Model));

        DataService.CommitTransaction();
        DataService.BeginTransaction();
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
        CommitItems();
        ItemsView.Refresh();
    }

    private void UpdateSelected()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        Selected = ItemsView.OfType<TriggerViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Trigger model = new();
        TriggerService.Insert(model);

        TriggerViewModel observable = new() { Model = model };

        Items.Add(observable);
        Selected = observable;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(object? param)
    {
        if (param is not TriggerViewModel observable)
        { return; }

        CommitItems();

        Trigger model = observable.Model;
        TriggerService.Delete(model);

        Items.Remove(observable);
    }
    private bool CanDelete(object? param)
    {
        return param is TriggerViewModel;
    }
}

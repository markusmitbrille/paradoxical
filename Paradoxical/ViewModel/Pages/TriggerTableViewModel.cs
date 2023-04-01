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

public class TriggerTableViewModel : PageViewModel
    , IMessageHandler<SelectMessage>
    , IMessageHandler<SaveMessage>
{
    public override string PageName => "Triggers";

    public FinderViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public ITriggerService TriggerService { get; }

    private ObservableCollection<TriggerViewModel> items = new();
    public ObservableCollection<TriggerViewModel> Items
    {
        get => items;
        set => SetProperty(ref items, value);
    }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
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

    public TriggerTableViewModel(
        NavigationViewModel navigation,
        FinderViewModel finder,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        TriggerService = triggerService;
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
        base.OnNavigatedTo();

        Load();

        Mediator.Register<SelectMessage>(this);
        Mediator.Register<SaveMessage>(this);
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();

        Mediator.Unregister<SelectMessage>(this);
        Mediator.Unregister<SaveMessage>(this);
    }

    void IMessageHandler<SelectMessage>.Handle(SelectMessage message)
    {
        if (message.Model is not Trigger model)
        { return; }

        Selected = Items.SingleOrDefault(viewmodel => viewmodel.Model == model);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    private RelayCommand? loadCommand;
    public RelayCommand LoadCommand => loadCommand ??= new(Load);

    private void Load()
    {
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
        Selected = view.Cast<TriggerViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Trigger model = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };
        TriggerService.Insert(model);

        TriggerViewModel observable = new() { Model = model };
        Items.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? duplicateCommand;
    public RelayCommand<TriggerViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = new(observable.Model);
        TriggerService.Insert(model);
    }
    private bool CanDuplicate(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        TriggerService.Delete(observable.Model);

        Items.Remove(observable);
    }
    private bool CanDelete(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? editCommand;
    public RelayCommand<TriggerViewModel> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        var model = observable.Model;

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<SelectMessage>(new(model));
    }
    private bool CanEdit(TriggerViewModel? observable)
    {
        return observable != null;
    }
}

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
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Triggers";

    public FinderViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public ITriggerService TriggerService { get; }

    private ObservableCollection<TriggerViewModel> wrappers = new();
    public ObservableCollection<TriggerViewModel> Wrappers
    {
        get => wrappers;
        set => SetProperty(ref wrappers, value);
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

        if (e.PropertyName == nameof(Wrappers))
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

        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();

        Mediator.Unregister<ShutdownMessage>(this);
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        Wrappers = new(TriggerService.Get().Select(model => new TriggerViewModel(model)));

        ICollectionView view = CollectionViewSource.GetDefaultView(Wrappers);
        view.Filter = Predicate;
    }

    private void Save()
    {
        TriggerService.UpdateAll(Wrappers.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not TriggerViewModel wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        if (ParadoxPattern.IdFilterRegex.FuzzyMatch(wrapper.Id.ToString(), Filter) == true)
        { return true; }

        if (ParadoxPattern.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter) == true)
        { return true; }

        if (ParadoxPattern.CodeFilterRegex.FuzzyMatch(wrapper.Code, Filter) == true)
        { return true; }

        if (ParadoxPattern.TooltipFilterRegex.FuzzyMatch(wrapper.Tooltip, Filter) == true)
        { return true; }

        if (ParadoxPattern.FilterRegex.FuzzyMatch(wrapper.Name, Filter) == true)
        { return true; }

        return false;
    }

    private void UpdateView()
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(Wrappers);
        view.Refresh();
    }

    private void UpdateSelected()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        ICollectionView view = CollectionViewSource.GetDefaultView(Wrappers);
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

        TriggerViewModel observable = new(model);
        Wrappers.Add(observable);
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

        Wrappers.Remove(observable);
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

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

public class TriggerTableViewModel : PageViewModelBase,
    IElementTableViewModel,
    IMessageHandler<SelectMessage>,
    IMessageHandler<DeselectMessage>,
    IMessageHandler<InsertMessage>,
    IMessageHandler<DeleteMessage>,
    IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Triggers";

    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public ITriggerService TriggerService { get; }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
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

    public TriggerTableViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        TriggerService = triggerService;

        Mediator.Register<SelectMessage>(this);
        Mediator.Register<DeselectMessage>(this);
        Mediator.Register<InsertMessage>(this);
        Mediator.Register<DeleteMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Triggers))
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

    public void Handle(SelectMessage message)
    {
        if (message.Element is not Trigger model)
        { return; }

        if (Navigation.CurrentPage != this)
        { return; }

        Selected = Triggers.SingleOrDefault(viewmodel => viewmodel.Model == model);
    }

    public void Handle(DeselectMessage message)
    {
        if (message.Element is not Trigger model)
        { return; }

        if (Navigation.CurrentPage != this)
        { return; }

        if (Selected?.Model == model)
        {
            Selected = null;
        }
    }

    public void Handle(InsertMessage message)
    {
        if (message.Model is not Trigger model)
        { return; }

        Triggers.Add(new(model));
    }

    public void Handle(DeleteMessage message)
    {
        if (message.Model is not Trigger model)
        { return; }

        var viewmodels = Triggers.Where(viewmodel => viewmodel.Model == model).ToArray();
        foreach (var viewmodel in viewmodels)
        {
            Triggers.Remove(viewmodel);
        }
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        Triggers = new(TriggerService.Get().Select(model => new TriggerViewModel(model)));

        ICollectionView view = CollectionViewSource.GetDefaultView(Triggers);
        view.Filter = Predicate;
    }

    private void Save()
    {
        TriggerService.UpdateAll(Triggers.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not TriggerViewModel viewmodel)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        if (ParadoxPattern.IdFilterRegex.FuzzyMatch(viewmodel.Id.ToString(), Filter) == true)
        { return true; }

        if (ParadoxPattern.NameFilterRegex.FuzzyMatch(viewmodel.Name, Filter) == true)
        { return true; }

        if (ParadoxPattern.CodeFilterRegex.FuzzyMatch(viewmodel.Code, Filter) == true)
        { return true; }

        if (ParadoxPattern.TooltipFilterRegex.FuzzyMatch(viewmodel.Tooltip, Filter) == true)
        { return true; }

        if (ParadoxPattern.FilterRegex.FuzzyMatch(viewmodel.Name, Filter) == true)
        { return true; }

        return false;
    }

    private void UpdateView()
    {
        ICollectionView view = CollectionViewSource.GetDefaultView(Triggers);
        view.Refresh();
    }

    private void UpdateSelected()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        ICollectionView view = CollectionViewSource.GetDefaultView(Triggers);
        Selected = view.Cast<TriggerViewModel>().FirstOrDefault();
    }

    private ObservableCollection<TriggerViewModel> triggers = new();
    public ObservableCollection<TriggerViewModel> Triggers
    {
        get => triggers;
        set => SetProperty(ref triggers, value);
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        Save();

        Finder.Items = TriggerService.Get().Select(model => new TriggerViewModel(model));
        Finder.Selected = Selected;
        Finder.NameFilter = Filter;

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<SelectMessage>(new(Finder.Selected.Model));
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Trigger model = new()
        {
            Name = $"trg_{Guid.NewGuid()}",
            Code = "# some trigger",
        };

        TriggerService.Insert(model);
    }

    private RelayCommand<TriggerViewModel>? duplicateCommand;
    public RelayCommand<TriggerViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Trigger model = new(viewmodel.Model);
        TriggerService.Insert(model);
    }
    private bool CanDuplicate(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        TriggerService.Delete(viewmodel.Model);
    }
    private bool CanDelete(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<TriggerViewModel>? editCommand;
    public RelayCommand<TriggerViewModel> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<SelectMessage>(new(viewmodel.Model));
    }
    private bool CanEdit(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }
}

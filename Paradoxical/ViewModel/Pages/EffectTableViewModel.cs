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

public class EffectTableViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
{
    public override string PageName => "Effects";

    public IEffectService EffectService { get; }

    private ObservableCollection<EffectViewModel> items = new();
    public ObservableCollection<EffectViewModel> Items
    {
        get => items;
        set => SetProperty(ref items, value);
    }

    private EffectViewModel? selected;
    public EffectViewModel? Selected
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

    public EffectTableViewModel(
        IShell shell,
        IMediatorService mediator,
        IEffectService effectService)
        : base(shell, mediator)
    {
        EffectService = effectService;
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
    }

    protected override void OnNavigatingFrom()
    {
        Save();

        Mediator.Unregister<SaveMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    private RelayCommand? loadCommand;
    public RelayCommand LoadCommand => loadCommand ??= new(Load);

    private void Load()
    {
        Items = new(EffectService.Get().Select(model => new EffectViewModel() { Model = model }));

        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        view.Filter = Predicate;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        EffectService.UpdateAll(Items.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not EffectViewModel wrapper)
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
        Selected = view.Cast<EffectViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Effect model = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };
        EffectService.Insert(model);

        EffectViewModel observable = new() { Model = model };
        Items.Add(observable);
    }

    private RelayCommand<EffectViewModel>? duplicateCommand;
    public RelayCommand<EffectViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = new(observable.Model);
        EffectService.Insert(model);
    }
    private bool CanDuplicate(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? deleteCommand;
    public RelayCommand<EffectViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        EffectService.Delete(observable.Model);

        Items.Remove(observable);
    }
    private bool CanDelete(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? editCommand;
    public RelayCommand<EffectViewModel> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        var model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEdit(EffectViewModel? observable)
    {
        return observable != null;
    }
}

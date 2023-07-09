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

public class EffectTableViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Effects";

    public IEffectService EffectService { get; }

    private ObservableCollection<EffectViewModel> items = new();
    public ObservableCollection<EffectViewModel> Items
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

            OnPropertyChanged();
        }
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count == 1)
        {
            EffectViewModel observable = e.NewItems.Cast<EffectViewModel>().Single();
            EffectService.Insert(observable.Model);
        }
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count == 1)
        {
            EffectViewModel observable = e.OldItems.Cast<EffectViewModel>().Single();
            EffectService.Delete(observable.Model);

            var historyPages = Shell.PageHistory.OfType<EffectDetailsViewModel>()
                .Where(page => page.Selected?.Model == observable.Model)
                .ToArray();

            var futurePages = Shell.PageFuture.OfType<EffectDetailsViewModel>()
                .Where(page => page.Selected?.Model == observable.Model)
                .ToArray();

            Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
            Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
        }
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
        Selected = null;
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
        Items.Add(new());
    }

    private RelayCommand<EffectViewModel>? deleteCommand;
    public RelayCommand<EffectViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

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

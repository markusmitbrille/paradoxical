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

public class ScriptTableViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Scripts";

    public IScriptService ScriptService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ObservableCollection<ScriptViewModel> items = new();
    public ObservableCollection<ScriptViewModel> Items
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
            ScriptViewModel observable = e.NewItems.Cast<ScriptViewModel>().Single();
            ScriptService.Insert(observable.Model);
        }
        if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count == 1)
        {
            ScriptViewModel observable = e.OldItems.Cast<ScriptViewModel>().Single();
            ScriptService.Delete(observable.Model);

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

    private ScriptViewModel? selected;
    public ScriptViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public ScriptTableViewModel(
        IShell shell,
        IMediatorService mediator,
        IScriptService scriptService)
        : base(shell, mediator)
    {
        ScriptService = scriptService;
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
        Items = new(ScriptService.Get().Select(model => new ScriptViewModel() { Model = model }));

        ICollectionView view = CollectionViewSource.GetDefaultView(Items);
        view.Filter = Predicate;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        ScriptService.UpdateAll(Items.Select(vm => vm.Model));
    }

    private bool Predicate(object obj)
    {
        if (obj is not ScriptViewModel wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        // feature filters

        bool?[] features = new[]
        {
            ParadoxPattern.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter),
            ParadoxPattern.DirFilterRegex.FuzzyMatch(wrapper.Dir, Filter),
            ParadoxPattern.FileFilterRegex.FuzzyMatch(wrapper.File, Filter),
            ParadoxPattern.CodeFilterRegex.FuzzyMatch(wrapper.Code, Filter),
        };

        if (features.Any(res => res == true) && features.All(res => res != false))
        { return true; }

        // general filter

        if (ParadoxPattern.FilterRegex.FuzzyMatch(wrapper.Path, Filter) == true)
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
        Selected = view.OfType<ScriptViewModel>().FirstOrDefault();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        ScriptViewModel item = new();

        Items.Add(item);
        Selected = item;
    }

    private RelayCommand<ScriptViewModel>? deleteCommand;
    public RelayCommand<ScriptViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        Items.Remove(observable);
    }
    private bool CanDelete(object? param)
    {
        return param is ScriptViewModel;
    }

    private RelayCommand<object>? editCommand;
    public RelayCommand<object> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        var model = observable.Model;

        var page = Shell.Navigate<ScriptDetailsViewModel>();
        page.Load(model);
    }
    private bool CanEdit(object? param)
    {
        return param is ScriptViewModel;
    }
}

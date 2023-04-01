using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class FinderViewModel : ObservableObject
{
    private bool? dialogResult;
    public bool? DialogResult
    {
        get => dialogResult;
        set => SetProperty(ref dialogResult, value);
    }

    private IEnumerable<IElementWrapper>? items;
    public IEnumerable<IElementWrapper> Items
    {
        get => items ?? Enumerable.Empty<IElementWrapper>();
        set => SetProperty(ref items, value);
    }

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);
    private IEnumerable<IElementWrapper> FilteredItems => View.Cast<IElementWrapper>();

    private string? filter;
    public string? Filter
    {
        get => filter;
        set => SetProperty(ref filter, value);
    }

    private IElementWrapper? selected;
    public IElementWrapper? Selected
    {
        get => selected;
        set
        {
            SetProperty(ref selected, value);
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    public async Task<object?> Show()
    {
        return await DialogHost.Show(this, MainWindow.ROOT_DIALOG_IDENTIFIER);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Items))
        {
            View.Filter = Predicate;

            UpdateView();
            UpdateSelection();
        }

        if (e.PropertyName == nameof(Filter))
        {
            UpdateView();
            UpdateSelection();
        }
    }

    private bool Predicate(object obj)
    {
        if (obj is not IElementWrapper wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        // feature filters

        bool?[] features = new[]
        {
            ParadoxPattern.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter),
            ParadoxPattern.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter),
            ParadoxPattern.TypeFilterRegex.FuzzyMatch(wrapper.Kind, Filter),
        };

        if (features.Any(res => res == true) && features.All(res => res != false))
        { return true; }

        // general filter

        if (ParadoxPattern.FilterRegex.FuzzyMatch(wrapper.Name, Filter) == true)
        { return true; }

        return false;
    }

    private RelayCommand? submitCommand;
    public RelayCommand SubmitCommand => submitCommand ??= new(Submit, CanSubmit);

    private void Submit()
    {
        DialogResult = true;

        Close();
    }
    private bool CanSubmit()
    {
        return true;
    }

    private RelayCommand? cancelCommand;
    public RelayCommand CancelCommand => cancelCommand ??= new(Cancel);

    private void Cancel()
    {
        DialogResult = false;

        Close();
    }

    private RelayCommand? closeCommand;
    public RelayCommand CloseCommand => closeCommand ??= new(Close);

    private void Close()
    {
        if (DialogHost.IsDialogOpen(MainWindow.ROOT_DIALOG_IDENTIFIER))
        { DialogHost.Close(MainWindow.ROOT_DIALOG_IDENTIFIER); }
    }

    private RelayCommand? updateViewCommand;
    public RelayCommand UpdateViewCommand => updateViewCommand ??= new(UpdateView);

    private void UpdateView()
    {
        View.Refresh();
    }

    private RelayCommand? updateSelectionCommand;
    public RelayCommand UpdateSelectionCommand => updateSelectionCommand ??= new(UpdateSelection);

    private void UpdateSelection()
    {
        if (Selected != null && Predicate(Selected) == true)
        { return; }

        Selected = FilteredItems.FirstOrDefault();
    }

    private RelayCommand? previousCommand;
    public RelayCommand PreviousCommand => previousCommand ??= new(Previous);

    private void Previous()
    {
        if (Selected == null)
        { return; }

        if (View == null)
        { return; }

        View.MoveCurrentToPrevious();
    }

    private RelayCommand? nextCommand;
    public RelayCommand NextCommand => nextCommand ??= new(Next);

    private void Next()
    {
        if (Selected == null)
        { return; }

        if (View == null)
        { return; }

        View.MoveCurrentToNext();
    }
}

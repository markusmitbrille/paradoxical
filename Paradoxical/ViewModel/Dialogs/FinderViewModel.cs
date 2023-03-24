using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class FinderViewModel : PopupViewModel
{
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

        if (ParadoxPattern.TypeFilterRegex.FuzzyMatch(wrapper.Kind, Filter) == false)
        { return false; }

        if (ParadoxPattern.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter) == false)
        { return false; }

        if (ParadoxPattern.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter) == false)
        { return false; }

        return true;
    }

    protected override bool CanSubmit()
    {
        return Selected != null;
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

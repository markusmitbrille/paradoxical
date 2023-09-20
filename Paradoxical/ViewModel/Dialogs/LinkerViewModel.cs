using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class LinkerViewModel : DialogViewModel
{
    private string scope = string.Empty;
    public string Scope
    {
        get => scope;
        set => SetProperty(ref scope, value);
    }

    private int minDays = 0;
    public int MinDays
    {
        get => minDays;
        set => SetProperty(ref minDays, value);
    }

    private int maxDays = 0;
    public int MaxDays
    {
        get => maxDays;
        set => SetProperty(ref maxDays, value);
    }

    private IEnumerable<ISearchable>? items;
    public IEnumerable<ISearchable> Items
    {
        get => items ?? Enumerable.Empty<ISearchable>();
        set
        {
            SetProperty(ref items, value);

            View.Filter = Predicate;

            UpdateView();
            UpdateSelection();
        }
    }

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);

    private ISearchable? selected;
    public ISearchable? Selected
    {
        get => selected;
        set
        {
            SetProperty(ref selected, value);
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private string? filter;
    public string? Filter
    {
        get => filter;
        set
        {
            SetProperty(ref filter, value);

            UpdateView();
            UpdateSelection();
        }
    }

    private bool Predicate(object obj)
    {
        if (obj is not ISearchable searchable)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        return searchable.Filter(Filter);
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
        if (Selected == null || Predicate(Selected) != true || Items.Contains(Selected) == false)
        {
            Selected = View.Cast<ISearchable>().FirstOrDefault();
        }
    }

    private RelayCommand? previousCommand;
    public RelayCommand PreviousCommand => previousCommand ??= new(Previous);

    private void Previous()
    {
        if (Selected == null)
        { return; }

        View.MoveCurrentToPrevious();

        KeepWithinView();
    }

    private RelayCommand? nextCommand;
    public RelayCommand NextCommand => nextCommand ??= new(Next);

    private void Next()
    {
        if (Selected == null)
        { return; }

        View.MoveCurrentToNext();

        KeepWithinView();
    }

    private void KeepWithinView()
    {
        if (View.IsCurrentBeforeFirst)
        {
            View.MoveCurrentToFirst();
        }

        if (View.IsCurrentAfterLast)
        {
            View.MoveCurrentToLast();
        }
    }
}

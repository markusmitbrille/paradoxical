using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public static partial class FinderRegex
{
    [GeneratedRegex(@"(?<filter>.+)")]
    private static partial Regex GetFilterRegex();
    public static Regex FilterRegex => GetFilterRegex();

    [GeneratedRegex(@"type:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTypeFilterRegex();
    public static Regex TypeFilterRegex => GetTypeFilterRegex();

    [GeneratedRegex(@"id:(?<filter>\d+)")]
    private static partial Regex GetIdFilterRegex();
    public static Regex IdFilterRegex => GetIdFilterRegex();

    [GeneratedRegex(@"name:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetNameFilterRegex();
    public static Regex NameFilterRegex => GetNameFilterRegex();
}

public class FinderViewModel : DialogViewModel
{
    private IEnumerable<IElementWrapper>? items;
    public IEnumerable<IElementWrapper> Items
    {
        get => items ??= Enumerable.Empty<IElementWrapper>();
        set
        {
            SetProperty(ref items, value);

            View.Filter = Predicate;

            UpdateView();
            UpdateSelection();
        }
    }

    private ICollectionView View => CollectionViewSource.GetDefaultView(Items);
    private IEnumerable<IElementWrapper> FilteredItems => View.Cast<IElementWrapper>();

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
        if (obj is not IElementWrapper wrapper)
        { return false; }

        if (string.IsNullOrEmpty(Filter) == true)
        { return true; }

        // feature filters

        bool?[] features = new[]
        {
            FinderRegex.IdFilterRegex.ExactMatch(wrapper.Id.ToString(), Filter),
            FinderRegex.NameFilterRegex.FuzzyMatch(wrapper.Name, Filter),
            FinderRegex.TypeFilterRegex.FuzzyMatch(wrapper.Kind, Filter),
        };

        if (features.Any(res => res == true) && features.All(res => res != false))
        { return true; }

        // general filter

        if (FinderRegex.FilterRegex.FuzzyMatch(wrapper.Name, Filter) == true)
        { return true; }

        return false;
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

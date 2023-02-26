using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class FindDialogViewModel : DialogViewModelBase
{
    public ICollectionView View { get; }

    private string? nameFilter;
    public string? NameFilter
    {
        get => nameFilter;
        set => SetProperty(ref nameFilter, value);
    }

    private Type? typeFilter;
    public Type? TypeFilter
    {
        get => typeFilter;
        set => SetProperty(ref typeFilter, value);
    }

    private IElementViewModel? selected;
    public IElementViewModel? Selected
    {
        get => selected;
        set
        {
            SetProperty(ref selected, value);
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    public FindDialogViewModel(IEnumerable<IElementViewModel> items)
    {
        View = CollectionViewSource.GetDefaultView(items);
        View.Filter = FilterItems;

        PropertyChanged += NameFilterPropertyChangedHandler;
        PropertyChanged += TypeFilterPropertyChangedHandler;
    }

    private void NameFilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(NameFilter))
        { return; }

        View?.Refresh();
        Selected = View?.Cast<IElementViewModel>().FirstOrDefault();
    }

    private void TypeFilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TypeFilter))
        { return; }

        View?.Refresh();
        Selected = View?.Cast<IElementViewModel>().FirstOrDefault();
    }

    private bool FilterItems(object obj)
    {
        if (obj is not IElementViewModel element)
        { return false; }

        if (TypeFilter != null && element.GetType() != TypeFilter)
        { return false; }

        if (string.IsNullOrEmpty(NameFilter) == false && element.Name.Contains(NameFilter, StringComparison.InvariantCultureIgnoreCase) == false)
        { return false; }

        return true;
    }

    protected override bool CanSubmit()
    {
        return Selected != null;
    }

    private RelayCommand? updateSelectionCommand;
    public RelayCommand UpdateSelectionCommand => updateSelectionCommand ??= new(UpdateSelection);

    private void UpdateSelection()
    {
        if (Selected == null && View != null)
        {
            Selected = View.Cast<IElementViewModel>().FirstOrDefault();
        }
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
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel;

public class FindDialogViewModel : FullScreenDialogViewModelBase
{
    public IEnumerable<IElementViewModel> Items
    {
        set
        {
            View = CollectionViewSource.GetDefaultView(value);
            View.Filter = Predicate;
        }
    }

    private ICollectionView? view;
    public ICollectionView? View
    {
        get => view;
        set => SetProperty(ref view, value);
    }

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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(View))
        {
            UpdateView();
            UpdateSelected();
        }

        if (e.PropertyName == nameof(NameFilter))
        {
            UpdateView();
            UpdateSelected();
        }

        if (e.PropertyName == nameof(TypeFilter))
        {
            UpdateView();
            UpdateSelected();
        }
    }

    private void UpdateView()
    {
        if (View == null)
        { return; }

        View.Refresh();
    }

    private void UpdateSelected()
    {
        if (View == null)
        { return; }

        if (Selected == null)
        { return; }

        if (Predicate(Selected) == true)
        { return; }

        Selected = View.Cast<IElementViewModel>().FirstOrDefault();
    }

    private bool Predicate(object obj)
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
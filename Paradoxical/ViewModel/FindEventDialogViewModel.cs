using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindEventDialogViewModel : ObservableObject
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent>? blacklist;

        [ObservableProperty]
        private ParadoxEvent? selected;

        public FindEventDialogViewModel()
        {
            PropertyChanged += ItemsPropertyChangedHandler;
            PropertyChanged += FilterPropertyChangedHandler;
            PropertyChanged += BlacklistPropertyChangedHandler;
        }

        private void BlacklistPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Blacklist))
            { return; }

            view?.Refresh();

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            if (view == null)
            { return; }

            view.Refresh();
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            Selected = view.Cast<ParadoxEvent>().FirstOrDefault();
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ParadoxEvent evt)
            { return false; }

            if (Blacklist != null && Blacklist.Contains(evt))
            { return false; }

            if (string.IsNullOrEmpty(Filter))
            { return true; }

            if (evt.Id.ToString().Contains(Filter))
            { return true; }

            if (evt.Title.ToLowerInvariant().Contains(Filter.ToLowerInvariant()))
            { return true; }

            return false;
        }

        [RelayCommand]
        private void Close()
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
            { DialogHost.Close("RootDialog"); }
        }

        [RelayCommand]
        private void UpdateSelection()
        {
            if (Selected == null && view != null)
            {
                Selected = view.Cast<ParadoxEvent>().FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            List<ParadoxEvent> filteredItems = new(view.Cast<ParadoxEvent>());

            if (Selected == filteredItems.First())
            { return; }

            int index = filteredItems.IndexOf(Selected);
            Selected = filteredItems[index - 1];
        }

        [RelayCommand]
        private void Next()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            List<ParadoxEvent> filteredItems = new(view.Cast<ParadoxEvent>());

            if (Selected == filteredItems.Last())
            { return; }

            int index = filteredItems.IndexOf(Selected);
            Selected = filteredItems[index + 1];
        }
    }
}
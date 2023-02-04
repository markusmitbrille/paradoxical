using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindTriggerDialogViewModel : ObservableObject
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger>? blacklist;

        [ObservableProperty]
        private ParadoxTrigger? selected;

        public FindTriggerDialogViewModel()
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
            Selected = (ParadoxTrigger?)(view?.CurrentItem);

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
            Selected = (ParadoxTrigger?)(view?.CurrentItem);
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            if (view == null)
            { return; }

            view.Refresh();
            Selected = (ParadoxTrigger?)(view?.CurrentItem);
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            view?.Refresh();
            Selected = (ParadoxTrigger?)(view?.CurrentItem);
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ParadoxTrigger trg)
            { return false; }

            if (Blacklist != null && Blacklist.Contains(trg))
            { return false; }

            if (string.IsNullOrEmpty(Filter))
            { return true; }

            if (trg.Name.ToString().Contains(Filter))
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
                Selected = view.Cast<ParadoxTrigger>().FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            List<ParadoxTrigger> filteredItems = new(view.Cast<ParadoxTrigger>());

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

            List<ParadoxTrigger> filteredItems = new(view.Cast<ParadoxTrigger>());

            if (Selected == filteredItems.Last())
            { return; }

            int index = filteredItems.IndexOf(Selected);
            Selected = filteredItems[index + 1];
        }
    }
}
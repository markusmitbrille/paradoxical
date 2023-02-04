using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindEffectDialogViewModel : ObservableObject
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ParadoxEffect? selected;

        public FindEffectDialogViewModel()
        {
            PropertyChanged += ItemsPropertyChangedHandler;
            PropertyChanged += FilterPropertyChangedHandler;
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
        }

        private bool FilterItems(object obj)
        {
            if (string.IsNullOrEmpty(Filter))
            { return true; }

            if (obj is not ParadoxEffect evt)
            { return false; }

            if (evt.Name.ToString().Contains(Filter))
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
                Selected = view.Cast<ParadoxEffect>().FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            List<ParadoxEffect> filteredItems = new(view.Cast<ParadoxEffect>());

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

            List<ParadoxEffect> filteredItems = new(view.Cast<ParadoxEffect>());

            if (Selected == filteredItems.Last())
            { return; }

            int index = filteredItems.IndexOf(Selected);
            Selected = filteredItems[index + 1];
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindEventDialogViewModel : ObservableObject
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent>? items;

        [ObservableProperty]
        private string filter = "";

        [ObservableProperty]
        private ParadoxEvent? selected;

        public FindEventDialogViewModel()
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
            if (obj is not ParadoxEvent evt)
            { return false; }

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
    }
}
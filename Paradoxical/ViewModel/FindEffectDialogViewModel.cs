using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindEffectDialogViewModel : DialogViewModelBase
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect>? blacklist;

        [ObservableProperty]
        private ParadoxEffect? selected;

        public FindEffectDialogViewModel()
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
            Selected = (ParadoxEffect?)(view?.CurrentItem);

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
            Selected = (ParadoxEffect?)(view?.CurrentItem);
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            if (view == null)
            { return; }

            view?.Refresh();
            Selected = (ParadoxEffect?)(view?.CurrentItem);
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            view?.Refresh();
            Selected = (ParadoxEffect?)(view?.CurrentItem);
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ParadoxEffect eff)
            { return false; }

            if (Blacklist != null && Blacklist.Contains(eff))
            { return false; }

            if (string.IsNullOrEmpty(Filter))
            { return true; }

            if (eff.Name.ToString().Contains(Filter))
            { return true; }

            return false;
        }

        protected override bool CanSubmit()
        {
            return Selected != null;
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
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
    public partial class FindOnActionDialogViewModel : DialogViewModelBase
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxOnAction>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ObservableCollection<ParadoxOnAction>? blacklist;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private ParadoxOnAction? selected;

        public FindOnActionDialogViewModel()
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
            Selected = view?.Cast<ParadoxOnAction>().FirstOrDefault();

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
            Selected = view?.Cast<ParadoxOnAction>().FirstOrDefault();
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            if (view == null)
            { return; }

            view?.Refresh();
            Selected = view?.Cast<ParadoxOnAction>().FirstOrDefault();
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            view?.Refresh();
            Selected = view?.Cast<ParadoxOnAction>().FirstOrDefault();
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ParadoxOnAction act)
            { return false; }

            if (Blacklist != null && Blacklist.Contains(act))
            { return false; }

            if (string.IsNullOrEmpty(Filter))
            { return true; }

            if (act.Name.ToString().Contains(Filter))
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
                Selected = view.Cast<ParadoxOnAction>().FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            List<ParadoxOnAction> filteredItems = new(view.Cast<ParadoxOnAction>());

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

            List<ParadoxOnAction> filteredItems = new(view.Cast<ParadoxOnAction>());

            if (Selected == filteredItems.Last())
            { return; }

            int index = filteredItems.IndexOf(Selected);
            Selected = filteredItems[index + 1];
        }
    }
}
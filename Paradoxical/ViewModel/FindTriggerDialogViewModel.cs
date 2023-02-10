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
    public partial class FindTriggerDialogViewModel : DialogViewModelBase
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger>? blacklist;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
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
            Selected = view?.Cast<ParadoxTrigger>().FirstOrDefault();

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
            Selected = view?.Cast<ParadoxTrigger>().FirstOrDefault();
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            if (view == null)
            { return; }

            view.Refresh();
            Selected = view?.Cast<ParadoxTrigger>().FirstOrDefault();
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            view?.Refresh();
            Selected = view?.Cast<ParadoxTrigger>().FirstOrDefault();
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

        protected override bool CanSubmit()
        {
            return Selected != null;
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
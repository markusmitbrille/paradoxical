using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Model;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Paradoxical.ViewModel
{
    public partial class FindDialogViewModel : DialogViewModelBase
    {
        private ICollectionView? view;

        [ObservableProperty]
        private ObservableCollection<ParadoxElement>? items;

        [ObservableProperty]
        private string? filter;

        [ObservableProperty]
        private Type elementType;

        [ObservableProperty]
        private ObservableCollection<ParadoxElement>? blacklist;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private ParadoxElement? selected;

        public FindDialogViewModel()
        {
            PropertyChanged += ItemsPropertyChangedHandler;
            PropertyChanged += FilterPropertyChangedHandler;
            PropertyChanged += ElementTypePropertyChangedHandler;
            PropertyChanged += BlacklistPropertyChangedHandler;
        }

        private void BlacklistPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Blacklist))
            { return; }

            view?.Refresh();
            Selected = view?.Cast<ParadoxElement>().FirstOrDefault();

            if (Blacklist != null)
            {
                Blacklist.CollectionChanged += BlacklistCollectionChangedHandler;
            }
        }

        private void BlacklistCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            view?.Refresh();
            Selected = view?.Cast<ParadoxElement>().FirstOrDefault();
        }

        private void FilterPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Filter))
            { return; }

            view?.Refresh();
            Selected = view?.Cast<ParadoxElement>().FirstOrDefault();
        }

        private void ElementTypePropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ElementType))
            { return; }

            view?.Refresh();
            Selected = view?.Cast<ParadoxElement>().FirstOrDefault();
        }

        private void ItemsPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Items))
            { return; }

            view = CollectionViewSource.GetDefaultView(Items);
            view.Filter = FilterItems;

            view?.Refresh();
            Selected = view?.Cast<ParadoxElement>().FirstOrDefault();
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ParadoxElement element)
            { return false; }

            if (ElementType != null && element.GetType() != ElementType)
            { return false; }

            if (Blacklist != null && Blacklist.Contains(element))
            { return false; }

            if (string.IsNullOrEmpty(Filter) == false && element.Name.ToString().Contains(Filter) == false)
            { return false; }

            return true;
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
                Selected = view.Cast<ParadoxElement>().FirstOrDefault();
            }
        }

        [RelayCommand]
        private void Previous()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            view.MoveCurrentToPrevious();
        }

        [RelayCommand]
        private void Next()
        {
            if (Selected == null)
            { return; }

            if (view == null)
            { return; }

            view.MoveCurrentToNext();
        }
    }
}
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Paradoxical.ViewModel;

public delegate PageViewModelBase PageFactory(Type pageType);

public class NavigationViewModel : ViewModelBase
{
    private PageFactory PageFactory { get; }

    private PageViewModelBase? currentPage;
    public PageViewModelBase? CurrentPage
    {
        get => currentPage;
        private set => SetProperty(ref currentPage, value);
    }

    private readonly Stack<PageViewModelBase> history = new();
    private readonly Stack<PageViewModelBase> future = new();

    public event Action? Navigating;
    public event Action? Navigated;

    public NavigationViewModel(PageFactory pageFactory)
    {
        PageFactory = pageFactory;
    }

    protected override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);

        if (e.PropertyName == nameof(CurrentPage))
        {
            Navigating?.Invoke();
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentPage))
        {
            Navigated?.Invoke();
        }
    }

    private RelayCommand? goForwardCommand;
    public RelayCommand GoForwardCommand => goForwardCommand ??= new(GoForward);

    public void GoForward()
    {
        if (CurrentPage != null)
        {
            history.Push(CurrentPage);
        }

        CurrentPage = future.Pop();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanGoForward()
    {
        return future.Any();
    }

    private RelayCommand? goBackCommand;
    public RelayCommand GoBackCommand => goBackCommand ??= new(GoBack);

    public void GoBack()
    {
        if (CurrentPage != null)
        {
            future.Push(CurrentPage);
        }

        CurrentPage = history.Pop();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanGoBack()
    {
        return history.Any();
    }

    private RelayCommand? goHomeCommand;
    public RelayCommand GoHomeCommand => goHomeCommand ??= new(GoHome);

    public void GoHome()
    {
        history.Clear();
        future.Clear();

        CurrentPage = null;

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }

    private RelayCommand<PageViewModelBase?>? navigateCommand;
    public RelayCommand<PageViewModelBase?> NavigateCommand => navigateCommand ??= new(Navigate, CanNavigate);

    public void Navigate(PageViewModelBase? page)
    {
        if (page == null)
        { return; }

        future.Clear();
        if (CurrentPage != null)
        {
            history.Push(CurrentPage);
        }

        CurrentPage = page;

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanNavigate(PageViewModelBase? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    public void Navigate<TPage>() where TPage : PageViewModelBase
    {
        PageViewModelBase page = PageFactory(typeof(TPage));

        if (CanNavigate(page) == true)
        {
            Navigate(page);
        }
    }
}

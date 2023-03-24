using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Paradoxical.ViewModel;

public delegate PageViewModel PageFactory(Type pageType);

public class NavigationViewModel : ObservableObject
{
    private PageFactory PageFactory { get; }
    public IMediatorService Mediator { get; }

    private PageViewModel? currentPage;
    public PageViewModel? CurrentPage
    {
        get => currentPage;
        private set => SetProperty(ref currentPage, value);
    }

    private readonly List<PageViewModel> history = new();
    private readonly List<PageViewModel> future = new();

    public event Action? Navigating;
    public event Action? Navigated;

    public NavigationViewModel(
        PageFactory pageFactory,
        IMediatorService mediator)
    {
        PageFactory = pageFactory;
        Mediator = mediator;
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
    public RelayCommand GoForwardCommand => goForwardCommand ??= new(GoForward, CanGoForward);

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
    public RelayCommand GoBackCommand => goBackCommand ??= new(GoBack, CanGoBack);

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

    private RelayCommand<PageViewModel?>? navigateCommand;
    public RelayCommand<PageViewModel?> NavigateCommand => navigateCommand ??= new(Navigate, CanNavigate);

    public void Navigate(PageViewModel? page)
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
    public bool CanNavigate(PageViewModel? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    public void Navigate(Type pageType)
    {
        if (pageType.IsAssignableTo(typeof(PageViewModel)) == false)
        { throw new ArgumentException($"{pageType} must be assignable to {typeof(PageViewModel)}!", nameof(pageType)); }

        PageViewModel page = PageFactory(pageType);

        if (CanNavigate(page) == true)
        {
            Navigate(page);
        }
    }
    public void Navigate<T>() where T : PageViewModel
    {
        Navigate(typeof(T));
    }
}

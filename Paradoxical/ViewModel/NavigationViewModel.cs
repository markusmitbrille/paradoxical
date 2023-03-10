using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Paradoxical.ViewModel;

public delegate PageViewModelBase PageFactory(Type pageType);

public class NavigationViewModel : ViewModelBase,
    IMessageHandler<ElementDeletedMessage>
{
    private PageFactory PageFactory { get; }
    public IMediatorService Mediator { get; }

    private PageViewModelBase? currentPage;
    public PageViewModelBase? CurrentPage
    {
        get => currentPage;
        private set => SetProperty(ref currentPage, value);
    }

    private readonly List<PageViewModelBase> history = new();
    private readonly List<PageViewModelBase> future = new();

    public event Action? Navigating;
    public event Action? Navigated;

    public NavigationViewModel(
        PageFactory pageFactory,
        IMediatorService mediator)
    {
        PageFactory = pageFactory;
        Mediator = mediator;

        Mediator.Register<ElementDeletedMessage>(this);
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

    public void Handle(ElementDeletedMessage message)
    {
        CleanCollection(history);
        CleanCollection(future);

        CleanSelection();

        void CleanCollection(IList<PageViewModelBase> collection)
        {
            var pages = collection.OfType<IElementDetailsViewModel>()
                .Where(page => page.Selected?.Model == message.Model)
                .Cast<PageViewModelBase>()
                .ToArray();

            foreach (var page in pages)
            {
                collection.Remove(page);
            }
        }

        void CleanSelection()
        {
            if (CurrentPage is not IElementDetailsViewModel page)
            { return; }

            if (page.Selected?.Model != message.Model)
            { return; }

            CurrentPage = null;

            if (CanGoBack() == true)
            {
                GoBack();
                return;
            }

            if (CanGoForward() == true)
            {
                GoForward();
                return;
            }

            GoHome();
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

    public void Navigate(Type pageType)
    {
        if (pageType.IsAssignableTo(typeof(PageViewModelBase)) == false)
        { throw new ArgumentException($"{pageType} must be assignable to {typeof(PageViewModelBase)}!", nameof(pageType)); }

        PageViewModelBase page = PageFactory(pageType);

        if (CanNavigate(page) == true)
        {
            Navigate(page);
        }
    }
    public void Navigate<TPage>() where TPage : PageViewModelBase
    {
        Navigate(typeof(TPage));
    }
}

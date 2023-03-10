using Paradoxical.ViewModel;

namespace Paradoxical.Core;

public abstract class PageViewModelBase : ViewModelBase
{
    public abstract string PageName { get; }

    public NavigationViewModel Navigation { get; }

    public PageViewModelBase(NavigationViewModel navigation)
    {
        Navigation = navigation;

        navigation.Navigating += NavigatingHandler;
        navigation.Navigated += NavigatedHandler;
    }

    private void NavigatingHandler()
    {
        if (Navigation.CurrentPage == this)
        {
            OnNavigatingFrom();
        }
    }

    private void NavigatedHandler()
    {
        if (Navigation.CurrentPage == this)
        {
            OnNavigatedTo();
        }
    }

    protected virtual void OnNavigatingFrom()
    {
    }

    protected virtual void OnNavigatedTo()
    {
    }
}

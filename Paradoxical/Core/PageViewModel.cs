using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Messages;
using Paradoxical.Services;
using Paradoxical.ViewModel;
using System;

namespace Paradoxical.Core;

public abstract class PageViewModel : ObservableObject
{
    public IShell Shell { get; }
    public IMediatorService Mediator { get; }

    public abstract string PageName { get; }

    public PageViewModel(
        IShell shell,
        IMediatorService mediator)
    {
        Shell = shell;
        Mediator = mediator;

        Shell.Navigating += NavigatingHandler;
        Shell.Navigated += NavigatedHandler;
    }

    private void NavigatedHandler(object? sender, EventArgs e)
    {
        if (Shell.CurrentPage == this)
        {
            OnNavigatedTo();
        }
    }

    private void NavigatingHandler(object? sender, EventArgs e)
    {
        if (Shell.CurrentPage == this)
        {
            OnNavigatingFrom();
        }
    }

    protected virtual void OnNavigatingFrom()
    {
    }

    protected virtual void OnNavigatedTo()
    {
    }
}

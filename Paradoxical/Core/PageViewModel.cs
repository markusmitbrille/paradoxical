﻿using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Messages;
using Paradoxical.Services;
using Paradoxical.ViewModel;
using System;

namespace Paradoxical.Core;

public abstract class PageViewModel : ObservableObject
{
    public IShell Shell { get; }
    public IMediatorService Mediator { get; }

    public virtual string PageName => string.Empty;
    public virtual bool IsValid => true;

    public PageViewModel(
        IShell shell,
        IMediatorService mediator)
    {
        Shell = shell;
        Mediator = mediator;
    }

    public virtual void OnNavigatingFrom()
    {
    }

    public virtual void OnNavigatedTo()
    {
    }
}

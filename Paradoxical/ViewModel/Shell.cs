using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application = System.Windows.Application;

namespace Paradoxical.ViewModel;

public interface IShell
{
    public PageViewModel? CurrentPage { get; }

    public List<PageViewModel> PageHistory { get; }
    public List<PageViewModel> PageFuture { get; }

    public event EventHandler Navigating;
    public event EventHandler Navigated;

    public T Navigate<T>() where T : PageViewModel;
}

public class Shell : ObservableObject, IShell
{
    public IServiceProvider ServiceProvider { get; }

    public FinderViewModel Finder { get; }

    public IMediatorService Mediator { get; }
    public IFileService File { get; }

    public IEventService EventService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    private PageViewModel? currentPage;
    public PageViewModel? CurrentPage
    {
        get => currentPage;
        private set
        {
            Navigating.Invoke(this, new());
            SetProperty(ref currentPage, value);
            Navigated.Invoke(this, new());
        }
    }

    public List<PageViewModel> PageHistory { get; } = new();
    public List<PageViewModel> PageFuture { get; } = new();

    public event EventHandler Navigating = delegate { };
    public event EventHandler Navigated = delegate { };

    public Shell(
        IServiceProvider serviceProvider,
        FinderViewModel finder,
        IMediatorService mediator,
        IFileService file,
        IEventService eventService,
        ITriggerService triggerService,
        IEffectService effectService)
    {
        ServiceProvider = serviceProvider;

        Finder = finder;

        Mediator = mediator;
        File = file;

        EventService = eventService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    private RelayCommand? newCommand;
    public RelayCommand NewCommand => newCommand ??= new(New);

    private void New()
    {
        File.New();
        GoHome();
    }

    private RelayCommand? openCommand;
    public RelayCommand OpenCommand => openCommand ??= new(Open);

    private void Open()
    {
        File.Open();
        GoHome();
    }

    private RelayCommand? exportCommand;
    public RelayCommand ExportCommand => exportCommand ??= new(Export);

    private void Export()
    {
        File.Export();
    }

    private RelayCommand? exportAsCommand;
    public RelayCommand ExportAsCommand => exportAsCommand ??= new(ExportAs);

    private void ExportAs()
    {
        File.ExportAs();
    }

    private RelayCommand? exitCommand;
    public RelayCommand ExitCommand => exitCommand ??= new(Exit);

    private void Exit()
    {
        Mediator.Send<ShutdownMessage>(new());
        Application.Current.Shutdown();
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        Mediator.Send<SaveMessage>(new());

        Finder.Items = Enumerable.Empty<IElementWrapper>()
            .Union(EventService.Get().Select(model => new EventViewModel() { Model = model }))
            .Union(TriggerService.Get().Select(model => new TriggerViewModel() { Model = model }))
            .Union(EffectService.Get().Select(model => new EffectViewModel() { Model = model }));

        Finder.Selected = null;

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        if (Finder.Selected is EventViewModel)
        {
            Event model = (Event)Finder.Selected.Model;

            EventDetailsViewModel page = Navigate<EventDetailsViewModel>();
            page.Load(model);
        }
        if (Finder.Selected is TriggerViewModel)
        {
            Trigger model = (Trigger)Finder.Selected.Model;

            TriggerDetailsViewModel page = Navigate<TriggerDetailsViewModel>();
            page.Load(model);
        }
        if (Finder.Selected is EffectViewModel)
        {
            Effect model = (Effect)Finder.Selected.Model;

            EffectDetailsViewModel page = Navigate<EffectDetailsViewModel>();
            page.Load(model);
        }
    }

    public T Navigate<T>() where T : PageViewModel
    {
        T page = ServiceProvider.GetRequiredService<T>();

        if (CanNavigate(page) == true)
        {
            Navigate(page);
        }

        return page;
    }

    private RelayCommand<PageViewModel?>? navigateCommand;
    public RelayCommand<PageViewModel?> NavigateCommand => navigateCommand ??= new(Navigate, CanNavigate);

    public void Navigate(PageViewModel? page)
    {
        if (page == null)
        { return; }

        PageFuture.Clear();
        if (CurrentPage != null)
        {
            PageHistory.Push(CurrentPage);
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

    private RelayCommand? goHomeCommand;
    public RelayCommand GoHomeCommand => goHomeCommand ??= new(GoHome);

    public void GoHome()
    {
        PageHistory.Clear();
        PageFuture.Clear();

        CurrentPage = null;

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }

    private RelayCommand? goBackCommand;
    public RelayCommand GoBackCommand => goBackCommand ??= new(GoBack, CanGoBack);

    public void GoBack()
    {
        if (CurrentPage != null)
        {
            PageFuture.Push(CurrentPage);
        }

        CurrentPage = PageHistory.Pop();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanGoBack()
    {
        return PageHistory.Any();
    }

    private RelayCommand? goForwardCommand;
    public RelayCommand GoForwardCommand => goForwardCommand ??= new(GoForward, CanGoForward);

    public void GoForward()
    {
        if (CurrentPage != null)
        {
            PageHistory.Push(CurrentPage);
        }

        CurrentPage = PageFuture.Pop();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanGoForward()
    {
        return PageFuture.Any();
    }

    private RelayCommand? goToInfoCommand;
    public RelayCommand GoToInfoCommand => goToInfoCommand ??= new(GoToInfo);

    private void GoToInfo()
    {
        Navigate<InfoViewModel>();
    }

    private RelayCommand? goToAboutCommand;
    public RelayCommand GoToAboutCommand => goToAboutCommand ??= new(GoToAbout);

    private void GoToAbout()
    {
        Navigate<AboutViewModel>();
    }

    private RelayCommand? goToEventTableCommand;
    public RelayCommand GoToEventTableCommand => goToEventTableCommand ??= new(GoToEventTable);

    private void GoToEventTable()
    {
        Navigate<EventTableViewModel>();
    }

    private RelayCommand? goToTriggerTableCommand;
    public RelayCommand GoToTriggerTableCommand => goToTriggerTableCommand ??= new(GoToTriggerTable);

    private void GoToTriggerTable()
    {
        Navigate<TriggerTableViewModel>();
    }

    private RelayCommand? goToEffectTableCommand;
    public RelayCommand GoToEffectTableCommand => goToEffectTableCommand ??= new(GoToEffectTable);

    private void GoToEffectTable()
    {
        Navigate<EffectTableViewModel>();
    }
}
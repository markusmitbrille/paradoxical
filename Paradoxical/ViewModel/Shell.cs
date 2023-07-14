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
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace Paradoxical.ViewModel;

public interface IShell
{
    public PageViewModel? CurrentPage { get; }

    public List<PageViewModel> PageHistory { get; }
    public List<PageViewModel> PageFuture { get; }

    public T Navigate<T>() where T : PageViewModel;

    public void GoHome();
    public void GoBack();
    public void GoForward();
}

public class Shell : ObservableObject, IShell
{
    public IServiceProvider ServiceProvider { get; }

    public IFinder Finder { get; }

    public IMediatorService Mediator { get; }
    public IDataService Data { get; }
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
            OnPropertyChanging();

            currentPage?.OnNavigatingFrom();
            currentPage = value;
            currentPage?.OnNavigatedTo();

            GoForwardCommand.NotifyCanExecuteChanged();
            GoBackCommand.NotifyCanExecuteChanged();

            OnPropertyChanged();
        }
    }

    public List<PageViewModel> PageHistory { get; } = new();
    public List<PageViewModel> PageFuture { get; } = new();

    public Shell(
        IServiceProvider serviceProvider,
        IFinder finder,
        IMediatorService mediator,
        IDataService data,
        IFileService file,
        IEventService eventService,
        ITriggerService triggerService,
        IEffectService effectService)
    {
        ServiceProvider = serviceProvider;

        Finder = finder;

        Mediator = mediator;
        Data = data;
        File = file;

        EventService = eventService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    private RelayCommand? newCommand;
    public RelayCommand NewCommand => newCommand ??= new(New);

    private void New()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        File.New();
        GoHome();
    }

    private RelayCommand? openCommand;
    public RelayCommand OpenCommand => openCommand ??= new(Open);

    private void Open()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        File.Open();
        GoHome();
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        Mediator.Send<SaveMessage>(new());
    }

    private RelayCommand? backupCommand;
    public RelayCommand BackupCommand => backupCommand ??= new(Backup);

    private void Backup()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        File.Backup();
        GoHome();
    }

    private RelayCommand? exportCommand;
    public RelayCommand ExportCommand => exportCommand ??= new(Export);

    private void Export()
    {
        Mediator.Send<SaveMessage>(new());
        File.Export();
    }

    private RelayCommand? exportAsCommand;
    public RelayCommand ExportAsCommand => exportAsCommand ??= new(ExportAs);

    private void ExportAs()
    {
        Mediator.Send<SaveMessage>(new());
        File.ExportAs();
    }

    private RelayCommand? exitCommand;
    public RelayCommand ExitCommand => exitCommand ??= new(Exit);

    private void Exit()
    {
        if (Data.IsInMemory != true)
        {
            Shutdown();
            return;
        }

        var result = MessageBox.Show(
@"You are currently working on an in-memory database, 
do you want to back it up? Your changes will be lost, 
if you don't save them.",
            "Database Backup",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            Backup();
            Shutdown();
        }
        if (result == MessageBoxResult.No)
        {
            Shutdown();
        }
    }

    private void Shutdown()
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
    }
    public bool CanNavigate(PageViewModel? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    public void ClearPage()
    {
        PageHistory.Clear();
        PageFuture.Clear();

        CurrentPage = null;
    }

    private RelayCommand? goHomeCommand;
    public RelayCommand GoHomeCommand => goHomeCommand ??= new(GoHome);

    public void GoHome()
    {
        GoToInfo();
    }

    private RelayCommand? goBackCommand;
    public RelayCommand GoBackCommand => goBackCommand ??= new(GoBack, CanGoBack);

    public void GoBack()
    {
        if (PageHistory.Any() == false)
        { return; }

        if (CurrentPage != null)
        {
            PageFuture.Push(CurrentPage);
        }

        CurrentPage = PageHistory.Pop();
    }
    public bool CanGoBack()
    {
        return PageHistory.Any() == true;
    }

    private RelayCommand? goForwardCommand;
    public RelayCommand GoForwardCommand => goForwardCommand ??= new(GoForward, CanGoForward);

    public void GoForward()
    {
        if (PageFuture.Any() == false)
        { return; }

        if (CurrentPage != null)
        {
            PageHistory.Push(CurrentPage);
        }

        CurrentPage = PageFuture.Pop();
    }
    public bool CanGoForward()
    {
        return PageFuture.Any() == true;
    }

    private RelayCommand? goToInfoCommand;
    public RelayCommand GoToInfoCommand => goToInfoCommand ??= new(GoToInfo);

    private void GoToInfo()
    {
        Navigate<ModDetailsViewModel>();
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
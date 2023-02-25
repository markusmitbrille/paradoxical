using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Paradoxical.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel;

public partial class ApplicationViewModel : ObservableObject
{
    public IDataService Data { get; }
    public BuildService Build { get; }

    public ApplicationViewModel(
        IDataService data,
        BuildService build,
        AboutPageViewModel aboutPage,
        InfoPageViewModel infoPage,
        EventPageViewModel eventPage,
        DecisionPageViewModel decisionPage,
        OnActionPageViewModel onActionPage,
        TriggerPageViewModel triggerPage,
        EffectPageViewModel effectPage,
        EventDetailsPageViewModel eventDetailsPage,
        DecisionDetailsPageViewModel decisionDetailsPage,
        OnActionDetailsPageViewModel onActionDetailsPage,
        TriggerDetailsPageViewModel triggerDetailsPage,
        EffectDetailsPageViewModel effectDetailsPage)
    {
        Data = data;
        Build = build;

        AboutPage = aboutPage;
        InfoPage = infoPage;

        EventPage = eventPage;
        DecisionPage = decisionPage;
        OnActionPage = onActionPage;
        TriggerPage = triggerPage;
        EffectPage = effectPage;

        EventDetailsPage = eventDetailsPage;
        DecisionDetailsPage = decisionDetailsPage;
        OnActionDetailsPage = onActionDetailsPage;
        TriggerDetailsPage = triggerDetailsPage;
        EffectDetailsPage = effectDetailsPage;

        Pages = new()
        {
            InfoPage,
            EventPage,
            DecisionPage,
            OnActionPage,
            TriggerPage,
            EffectPage,
            AboutPage,
        };

        // navigate to info page
        GoToPage(InfoPage);
    }

    public ObservableCollection<PageViewModelBase> Pages { get; }

    public AboutPageViewModel AboutPage { get; }
    public InfoPageViewModel InfoPage { get; }

    public EventPageViewModel EventPage { get; }
    public DecisionPageViewModel DecisionPage { get; }
    public OnActionPageViewModel OnActionPage { get; }
    public TriggerPageViewModel TriggerPage { get; }
    public EffectPageViewModel EffectPage { get; }

    public EventDetailsPageViewModel EventDetailsPage { get; }
    public DecisionDetailsPageViewModel DecisionDetailsPage { get; }
    public OnActionDetailsPageViewModel OnActionDetailsPage { get; }
    public TriggerDetailsPageViewModel TriggerDetailsPage { get; }
    public EffectDetailsPageViewModel EffectDetailsPage { get; }

    private PageViewModelBase? currentPage;
    public PageViewModelBase? CurrentPage
    {
        get => currentPage;
        set => SetProperty(ref currentPage, value);
    }

    private readonly Stack<PageViewModelBase> history = new();
    private readonly Stack<PageViewModelBase> future = new();

    private RelayCommand? navigateNextCommand;
    public RelayCommand NavigateNextCommand => navigateNextCommand ??= new(NavigateNext);

    public void NavigateNext()
    {
        if (CurrentPage != null)
        {
            history.Push(CurrentPage);
        }

        CurrentPage = future.Pop();

        NavigateNextCommand.NotifyCanExecuteChanged();
        NavigatePreviousCommand.NotifyCanExecuteChanged();
    }
    public bool CanNavigateNext()
    {
        return future.Any();
    }

    private RelayCommand? navigatePreviousCommand;
    public RelayCommand NavigatePreviousCommand => navigatePreviousCommand ??= new(NavigatePrevious);

    public void NavigatePrevious()
    {
        if (CurrentPage != null)
        {
            future.Push(CurrentPage);
        }

        CurrentPage = history.Pop();

        NavigateNextCommand.NotifyCanExecuteChanged();
        NavigatePreviousCommand.NotifyCanExecuteChanged();
    }
    public bool CanNavigatePrevious()
    {
        return history.Any();
    }

    private RelayCommand? goToNoneCommand;
    public RelayCommand GoToNoneCommand => goToNoneCommand ??= new(GoToNone);

    public void GoToNone()
    {
        history.Clear();
        future.Clear();

        CurrentPage = null;

        NavigateNextCommand.NotifyCanExecuteChanged();
        NavigatePreviousCommand.NotifyCanExecuteChanged();
    }

    private RelayCommand<PageViewModelBase?>? goToPageCommand;
    public RelayCommand<PageViewModelBase?> GoToPageCommand => goToPageCommand ??= new(GoToPage, CanGoToPage);

    public void GoToPage(PageViewModelBase? page)
    {
        if (page == null)
        { return; }

        future.Clear();
        if (CurrentPage != null)
        {
            history.Push(CurrentPage);
        }

        CurrentPage = page;

        NavigateNextCommand.NotifyCanExecuteChanged();
        NavigatePreviousCommand.NotifyCanExecuteChanged();
    }
    public bool CanGoToPage(PageViewModelBase? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    private string SaveDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string SaveFile { get; set; } = string.Empty;
    private string SavePath { get; set; } = string.Empty;

    private string ModDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private string ModFile { get; set; } = string.Empty;

    private RelayCommand? newModCommand;
    public RelayCommand NewModCommand => newModCommand ??= new(NewMod);

    private void NewMod()
    {
        SaveFileDialog dlg = new()
        {
            Title = "Create Mod",
            CreatePrompt = false,
            OverwritePrompt = true,
            Filter = "Paradoxical Mod|*.paradoxical",
            DefaultExt = ".paradoxical",
            AddExtension = true,
            InitialDirectory = SaveDir,
            FileName = SaveFile,
        };

        if (dlg.ShowDialog() != true)
        { return; }

        SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
        SavePath = dlg.FileName;

        if (Directory.Exists(SaveDir) == false)
        { return; }

        if (SaveFile == string.Empty)
        { return; }

        Data.Connect(SavePath);
        GoToNone();
    }

    private RelayCommand? openModCommand;
    public RelayCommand OpenModCommand => openModCommand ??= new(OpenMod);

    private void OpenMod()
    {
        OpenFileDialog dlg = new()
        {
            Title = "Open Mod",
            Filter = "Paradoxical Mod|*.paradoxical",
            DefaultExt = ".paradoxical",
            AddExtension = true,
            InitialDirectory = SaveDir,
            FileName = SaveFile,
        };

        if (dlg.ShowDialog() != true)
        { return; }

        SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
        SavePath = dlg.FileName;

        if (File.Exists(SavePath) == false)
        { return; }

        Data.Connect(SavePath);
        GoToNone();
    }

    private RelayCommand? buildModCommand;
    public RelayCommand BuildModCommand => buildModCommand ??= new(BuildMod);

    private void BuildMod()
    {
        if (Directory.Exists(ModDir) == false || ModFile == string.Empty)
        {
            BuildModAs();
            return;
        }

        Build.Export(ModDir, ModFile);
    }

    private RelayCommand? buildModAsCommand;
    public RelayCommand BuildModAsCommand => buildModAsCommand ??= new(BuildModAs);

    private void BuildModAs()
    {
        SaveFileDialog dlg = new()
        {
            Title = "Build Mod",
            CreatePrompt = false,
            OverwritePrompt = true,
            Filter = "Paradox Mod File|*.mod",
            DefaultExt = ".mod",
            AddExtension = true,
            InitialDirectory = ModDir,
            FileName = ModFile,
        };

        if (dlg.ShowDialog() != true)
        { return; }

        ModDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ModFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;

        if (Directory.Exists(ModDir) == false)
        { return; }

        if (ModFile == string.Empty)
        { return; }

        Build.Export(ModDir, ModFile);

        Process.Start("explorer.exe", ModDir);
    }

    private RelayCommand? exitCommand;
    public RelayCommand ExitCommand => exitCommand ??= new(Exit);

    private void Exit()
    {
        Application.Current.Shutdown();
    }
}
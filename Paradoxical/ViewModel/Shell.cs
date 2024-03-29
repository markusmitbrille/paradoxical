﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Entities;
using Paradoxical.Services;
using Paradoxical.Services.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace Paradoxical.ViewModel;

public interface IShell
{
    public void LoadConfig();
    public void SaveConfig();
    public void ApplyConfig();

    public PageViewModel? CurrentPage { get; }

    public T Navigate<T>() where T : PageViewModel;
    public void NavigatePage(PageViewModel? page);

    public void GoHome();
    public void ClearPage();
}

public class Shell : ObservableObject, IShell
{
    private const string WIKI_URL = "https://github.com/markusmitbrille/paradoxical/wiki";
    private const string EFFECT_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_effects.log";
    private const string TRIGGER_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_triggers.log";
    private const string SCOPE_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_event_targets.log";
    private const string ONION_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_on_actions.log";

    public IServiceProvider ServiceProvider { get; }

    public IMediatorService Mediator { get; }
    public IDataService DataService { get; }
    public IFileService FileService { get; }

    public Shell(
        IServiceProvider serviceProvider,
        IMediatorService mediator,
        IDataService dataService,
        IFileService fileService)
    {
        ServiceProvider = serviceProvider;

        Mediator = mediator;
        DataService = dataService;
        FileService = fileService;
    }

    #region Config

    private class ConfigData
    {
        public bool UseAltTheme { get; set; } = false;

        public string LastSavePath { get; set; } = string.Empty;
        public string LastModPath { get; set; } = string.Empty;
    }

    private const string CONFIG_DIR = "./Paradoxical/";
    private const string CONFIG_FILE = "config.json";

    private static string ConfigBase => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private static string ConfigDir => Path.Combine(ConfigBase, CONFIG_DIR);
    private static string ConfigPath => Path.Combine(ConfigDir, CONFIG_FILE);

    private ConfigData? config;
    private ConfigData Config
    {
        get => config ??= new();
        set => config = value;
    }

    public void LoadConfig()
    {
        if (Directory.Exists(ConfigDir) == false)
        {
            Directory.CreateDirectory(ConfigDir);
        }

        if (File.Exists(ConfigPath) == false)
        {
            CreateConfig();
            return;
        }

        ConfigData? config = null;

        try
        {
            string json = File.ReadAllText(ConfigPath);
            config = JsonSerializer.Deserialize<ConfigData>(json);
        }
        catch (JsonException)
        {
            Trace.TraceError("Could not deserialize config!");

            CreateConfig();
            return;
        }

        Config = config ?? new();
    }

    public void SaveConfig()
    {
        if (Directory.Exists(ConfigDir) == false)
        {
            Directory.CreateDirectory(ConfigDir);
        }

        string json = JsonSerializer.Serialize(Config);
        File.WriteAllText(ConfigPath, json);
    }

    public void ApplyConfig()
    {
        UseAltTheme = Config.UseAltTheme;

        if (File.Exists(Config.LastSavePath) == true)
        {
            FileService.SetSave(Config.LastSavePath);
            FileService.SetMod(Config.LastModPath);

            FileService.OpenSave();
        }
    }

    private void CreateConfig()
    {
        if (Directory.Exists(ConfigDir) == false)
        {
            Directory.CreateDirectory(ConfigDir);
        }

        Config = new();

        string json = JsonSerializer.Serialize(Config);
        File.WriteAllText(ConfigPath, json);
    }

    #endregion

    private RelayCommand? newCommand;
    public RelayCommand NewCommand => newCommand ??= new(New);

    private void New()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        FileService.ShowNewDialog();
        FileService.NewSave();
        GoHome();

        Config.LastSavePath = FileService.SavePath;
        Config.LastModPath = FileService.ModPath;
    }

    private RelayCommand? openCommand;
    public RelayCommand OpenCommand => openCommand ??= new(Open);

    private void Open()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        FileService.ShowOpenDialog();
        FileService.OpenSave();
        GoHome();

        Config.LastSavePath = FileService.SavePath;
        Config.LastModPath = FileService.ModPath;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (DataService.IsInMemory == true)
        {
            Backup();
            return;
        }

        Mediator.Send<SaveMessage>(new());
    }

    private RelayCommand? backupCommand;
    public RelayCommand BackupCommand => backupCommand ??= new(Backup);

    private void Backup()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        FileService.ShowBackupDialog();
        FileService.BackupSave();
        GoHome();

        Config.LastSavePath = FileService.SavePath;
        Config.LastModPath = FileService.ModPath;
    }

    private RelayCommand? exportCommand;
    public RelayCommand ExportCommand => exportCommand ??= new(Export);

    private void Export()
    {
        Mediator.Send<SaveMessage>(new());
        FileService.Export();

        Config.LastSavePath = FileService.SavePath;
        Config.LastModPath = FileService.ModPath;
    }

    private RelayCommand? exportAsCommand;
    public RelayCommand ExportAsCommand => exportAsCommand ??= new(ExportAs);

    private void ExportAs()
    {
        Mediator.Send<SaveMessage>(new());
        FileService.ExportAs();

        Config.LastSavePath = FileService.SavePath;
        Config.LastModPath = FileService.ModPath;
    }

    private RelayCommand? exitCommand;
    public RelayCommand ExitCommand => exitCommand ??= new(Exit);

    private void Exit()
    {
        if (DataService.IsInMemory != true)
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
        SaveConfig();

        Mediator.Send<ShutdownMessage>(new());
        Application.Current.Shutdown();
    }

    private RelayCommand? findCommand;
    public RelayCommand FindCommand => findCommand ??= new(Find);

    private void Find()
    {
        if (CurrentPage is not ContentPageViewModel)
        {
            GoToContent();
        }

        if (CurrentPage is not ContentPageViewModel page)
        { return; }

        page.Find();
    }

    #region Theme

    private bool? useAltTheme;
    public bool UseAltTheme
    {
        get => useAltTheme ??= false;
        set
        {
            SetProperty(ref useAltTheme, value);

            Config.UseAltTheme = UseAltTheme;

            if (value == false)
            {
                SetMainTheme();
            }
            else
            {
                SetAltTheme();
            }
        }
    }

    private static void SetMainTheme()
    {
        var paletteHelper = new PaletteHelper();

        ITheme theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(Theme.Light);

        paletteHelper.SetTheme(theme);
    }

    private static void SetAltTheme()
    {
        var paletteHelper = new PaletteHelper();

        ITheme theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(Theme.Dark);

        paletteHelper.SetTheme(theme);
    }

    #endregion

    #region Navigation

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

            OnPropertyChanged();
        }
    }

    public T Navigate<T>() where T : PageViewModel
    {
        T page = ServiceProvider.GetRequiredService<T>();

        if (CanNavigatePage(page) == true)
        {
            NavigatePage(page);
        }

        return page;
    }

    private RelayCommand<string>? navigateUrlCommand;
    public RelayCommand<string> NavigateUrlCommand => navigateUrlCommand ??= new(NavigateUrl);

    public static void NavigateUrl(string? url)
    {
        if (url == null)
        { return; }

        Process.Start(new ProcessStartInfo(url)
        {
            UseShellExecute = true,
            Verb = "open"
        });
    }

    private RelayCommand<PageViewModel?>? navigatePageCommand;
    public RelayCommand<PageViewModel?> NavigatePageCommand => navigatePageCommand ??= new(NavigatePage, CanNavigatePage);

    public void NavigatePage(PageViewModel? page)
    {
        if (page == null)
        { return; }

        CurrentPage = page;
    }
    public bool CanNavigatePage(PageViewModel? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    public void GoHome()
    {
        GoToContent();
    }

    public void ClearPage()
    {
        CurrentPage = null;
    }

    #endregion

    #region Page Commands

    private RelayCommand? goToContentCommand;
    public RelayCommand GoToContentCommand => goToContentCommand ??= new(GoToContent);

    private void GoToContent()
    {
        Navigate<ContentPageViewModel>();
    }

    private RelayCommand? goToOutputCommand;
    public RelayCommand GoToOutputCommand => goToOutputCommand ??= new(GoToOutput);

    private void GoToOutput()
    {
        Navigate<OutputPageViewModel>();
    }

    private RelayCommand? goToAboutCommand;
    public RelayCommand GoToAboutCommand => goToAboutCommand ??= new(GoToAbout);

    private void GoToAbout()
    {
        Navigate<AboutPageViewModel>();
    }

    private RelayCommand? goToWikiCommand;
    public RelayCommand GoToWikiCommand => goToWikiCommand ??= new(GoToWiki);

    private void GoToWiki()
    {
        NavigateUrl(WIKI_URL);
    }

    private RelayCommand? goToTriggerDocCommand;
    public RelayCommand GoToTriggerDocCommand => goToTriggerDocCommand ??= new(GoToTriggerDoc);

    private void GoToTriggerDoc()
    {
        NavigateUrl(TRIGGER_DOC_URL);
    }

    private RelayCommand? goToEffectDocCommand;
    public RelayCommand GoToEffectDocCommand => goToEffectDocCommand ??= new(GoToEffectDoc);

    private void GoToEffectDoc()
    {
        NavigateUrl(EFFECT_DOC_URL);
    }

    private RelayCommand? goToScopeDocCommand;
    public RelayCommand GoToScopeDocCommand => goToScopeDocCommand ??= new(GoToScopeDoc);

    private void GoToScopeDoc()
    {
        NavigateUrl(SCOPE_DOC_URL);
    }

    private RelayCommand? goToOnionDocCommand;
    public RelayCommand GoToOnionDocCommand => goToOnionDocCommand ??= new(GoToOnionDoc);

    private void GoToOnionDoc()
    {
        NavigateUrl(ONION_DOC_URL);
    }

    #endregion
}

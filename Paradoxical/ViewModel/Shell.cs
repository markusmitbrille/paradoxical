using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;
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

    public PageViewModel? CurrentPage { get; }

    public T Navigate<T>() where T : PageViewModel;

    public void NavigatePage(PageViewModel? page);
    public void InvalidatePage(PageViewModel? page);
    public void ValidatePages();

    public void ClearPage();
    public void GoHome();
    public void GoBack();
    public void GoForward();
}

public class Shell : ObservableObject, IShell
{
    private const string WIKI_URL = "https://github.com/markusmitbrille/paradoxical/wiki";
    private const string EFFECT_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_effects.log";
    private const string TRIGGER_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_triggers.log";
    private const string SCOPE_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_event_targets.log";
    private const string ONION_DOC_URL = "https://raw.githubusercontent.com/OldEnt/crusader-kings-3-triggers-modifiers-effects-event-scopes-targets-on-actions-code-revisions-list/master/1.9.0.4_on_actions.log";

    public IServiceProvider ServiceProvider { get; }

    public IFinder Finder { get; }

    public IMediatorService Mediator { get; }
    public IDataService DataService { get; }
    public IFileService FileService { get; }

    public IEventService EventService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    public Shell(
        IServiceProvider serviceProvider,
        IFinder finder,
        IMediatorService mediator,
        IDataService dataService,
        IFileService fileService,
        IEventService eventService,
        ITriggerService triggerService,
        IEffectService effectService)
    {
        ServiceProvider = serviceProvider;

        Finder = finder;

        Mediator = mediator;
        DataService = dataService;
        FileService = fileService;

        EventService = eventService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    #region Config

    private class ConfigData
    {
        public bool UseAltTheme { get; set; } = false;
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
        set
        {
            config = value;

            UseAltTheme = config.UseAltTheme;
        }
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
        FileService.New();
        GoHome();
    }

    private RelayCommand? openCommand;
    public RelayCommand OpenCommand => openCommand ??= new(Open);

    private void Open()
    {
        Mediator.Send<SaveMessage>(new());

        ClearPage();
        FileService.Open();
        GoHome();
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
        FileService.Backup();
        GoHome();
    }

    private RelayCommand? exportCommand;
    public RelayCommand ExportCommand => exportCommand ??= new(Export);

    private void Export()
    {
        Mediator.Send<SaveMessage>(new());
        FileService.Export();
    }

    private RelayCommand? exportAsCommand;
    public RelayCommand ExportAsCommand => exportAsCommand ??= new(ExportAs);

    private void ExportAs()
    {
        Mediator.Send<SaveMessage>(new());
        FileService.ExportAs();
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

            GoForwardCommand.NotifyCanExecuteChanged();
            GoBackCommand.NotifyCanExecuteChanged();

            OnPropertyChanged();
        }
    }

    private List<PageViewModel> History { get; } = new();
    private List<PageViewModel> Future { get; } = new();

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

        Future.Clear();

        if (CurrentPage != null && CurrentPage != History.PeekOrDefault())
        {
            History.Push(CurrentPage);
        }

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

    private RelayCommand<PageViewModel?>? invalidatePageCommand;
    public RelayCommand<PageViewModel?> InvalidatePageCommand => invalidatePageCommand ??= new(InvalidatePage, CanInvalidatePage);

    public void InvalidatePage(PageViewModel? page)
    {
        if (page == null)
        { return; }

        History.RemoveAll(page);
        Future.RemoveAll(page);

        RemoveDuplicatePages();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }
    public bool CanInvalidatePage(PageViewModel? page)
    {
        if (page == null)
        { return false; }

        if (CurrentPage == page)
        { return false; }

        return true;
    }

    private RelayCommand? validatePagesCommand;
    public RelayCommand ValidatePagesCommand => validatePagesCommand ??= new(ValidatePages);

    public void ValidatePages()
    {
        History.RemoveAll(page => page.IsValid == false);
        Future.RemoveAll(page => page.IsValid == false);

        RemoveDuplicatePages();

        GoForwardCommand.NotifyCanExecuteChanged();
        GoBackCommand.NotifyCanExecuteChanged();
    }

    private void RemoveDuplicatePages()
    {
        if (CurrentPage != null)
        {
            History.Push(CurrentPage);
            Future.Push(CurrentPage);
        }

        History.RemoveConsecutiveDuplicates();
        Future.RemoveConsecutiveDuplicates();

        if (CurrentPage != null)
        {
            History.Pop();
            Future.Pop();
        }
    }

    public void ClearPage()
    {
        History.Clear();
        Future.Clear();

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
        if (History.Any() == false)
        { return; }

        if (CurrentPage != null && CurrentPage != Future.PeekOrDefault())
        {
            Future.Push(CurrentPage);
        }

        CurrentPage = History.Pop();
    }
    public bool CanGoBack()
    {
        return History.Any() == true;
    }

    private RelayCommand? goForwardCommand;
    public RelayCommand GoForwardCommand => goForwardCommand ??= new(GoForward, CanGoForward);

    public void GoForward()
    {
        if (Future.Any() == false)
        { return; }

        if (CurrentPage != null && CurrentPage != History.PeekOrDefault())
        {
            History.Push(CurrentPage);
        }

        CurrentPage = Future.Pop();
    }
    public bool CanGoForward()
    {
        return Future.Any() == true;
    }

    #endregion

    #region Page Commands

    private RelayCommand? goToInfoCommand;
    public RelayCommand GoToInfoCommand => goToInfoCommand ??= new(GoToInfo);

    private void GoToInfo()
    {
        Navigate<ModDetailsViewModel>();
    }

    private RelayCommand? goToOutputCommand;
    public RelayCommand GoToOutputCommand => goToOutputCommand ??= new(GoToOutput);

    private void GoToOutput()
    {
        Navigate<OutputViewModel>();
    }

    private RelayCommand? goToAboutCommand;
    public RelayCommand GoToAboutCommand => goToAboutCommand ??= new(GoToAbout);

    private void GoToAbout()
    {
        Navigate<AboutViewModel>();
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

    private RelayCommand? goToEventTableCommand;
    public RelayCommand GoToEventTableCommand => goToEventTableCommand ??= new(GoToEventTable);

    private void GoToEventTable()
    {
        Navigate<EventTableViewModel>();
    }

    private RelayCommand? goToDecisionTableCommand;
    public RelayCommand GoToDecisionTableCommand => goToDecisionTableCommand ??= new(GoToDecisionTable);

    private void GoToDecisionTable()
    {
        Navigate<DecisionTableViewModel>();
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

    private RelayCommand? goToScriptTableCommand;
    public RelayCommand GoToScriptTableCommand => goToScriptTableCommand ??= new(GoToScriptTable);

    private void GoToScriptTable()
    {
        Navigate<ScriptTableViewModel>();
    }

    #endregion
}
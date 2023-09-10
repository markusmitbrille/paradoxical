using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System;
using System.Windows;

namespace Paradoxical;

public partial class App : Application
{
    private ServiceProvider ServiceProvider { get; }

    public App()
    {
        ServiceCollection services = new();

        // service provider
        // only shell should use this
        services.AddTransient(provider => provider);

        // main window and shell
        services.AddSingleton<IShell, Shell>();
        services.AddSingleton<MainWindow>();

        // general services
        services.AddSingleton<IBuildService, BuildService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IMediatorService, MediatorService>();

        // mod service
        services.AddSingleton<IModService, ModService>();

        // entity services
        services.AddSingleton<IScriptService, ScriptService>();
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IOptionService, OptionService>();
        services.AddSingleton<IOnionService, OnionService>();
        services.AddSingleton<IPortraitService, PortraitService>();
        services.AddSingleton<IDecisionService, DecisionService>();
        services.AddSingleton<ITriggerService, TriggerService>();
        services.AddSingleton<IEffectService, EffectService>();

        // finder view model
        services.AddTransient<IFinder, FinderViewModel>();

        // misc page view models
        services.AddSingleton<AboutPageViewModel>();

        // details view models
        services.AddSingleton<ScriptDetailsViewModel>();
        services.AddSingleton<EventDetailsViewModel>();
        services.AddSingleton<OptionDetailsViewModel>();
        services.AddSingleton<DecisionDetailsViewModel>();
        services.AddSingleton<TriggerDetailsViewModel>();
        services.AddSingleton<EffectDetailsViewModel>();

        // page view models
        services.AddSingleton<ModPageViewModel>();
        services.AddSingleton<OutputPageViewModel>();
        services.AddSingleton<ScriptPageViewModel>();
        services.AddSingleton<EventPageViewModel>();
        services.AddSingleton<DecisionPageViewModel>();
        services.AddSingleton<TriggerPageViewModel>();
        services.AddSingleton<EffectPageViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IShell shell = ServiceProvider.GetRequiredService<IShell>();
        shell.LoadConfig();

        MainWindow main = ServiceProvider.GetRequiredService<MainWindow>();
        main.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        // dispose of the service provider
        ServiceProvider.Dispose();
    }
}

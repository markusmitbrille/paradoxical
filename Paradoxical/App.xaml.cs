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
        services.AddSingleton<AboutViewModel>();

        // details page view models
        services.AddSingleton<ModDetailsViewModel>();
        services.AddSingleton<OutputViewModel>();
        services.AddTransient<ScriptDetailsViewModel>();
        services.AddTransient<EventDetailsViewModel>();
        services.AddTransient<OptionDetailsViewModel>();
        services.AddTransient<DecisionDetailsViewModel>();
        services.AddTransient<TriggerDetailsViewModel>();
        services.AddTransient<EffectDetailsViewModel>();

        // table page view models
        services.AddSingleton<ScriptTableViewModel>();
        services.AddSingleton<EventTableViewModel>();
        services.AddSingleton<DecisionTableViewModel>();
        services.AddSingleton<TriggerTableViewModel>();
        services.AddSingleton<EffectTableViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IShell shell = ServiceProvider.GetRequiredService<IShell>();
        shell.LoadConfig();

        MainWindow main = ServiceProvider.GetRequiredService<MainWindow>();
        main.Show();

        shell.GoHome();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        // dispose of the service provider
        ServiceProvider.Dispose();
    }
}

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

        // element services
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IOptionService, OptionService>();
        services.AddSingleton<IPortraitService, PortraitService>();
        services.AddSingleton<ITriggerService, TriggerService>();
        services.AddSingleton<IEffectService, EffectService>();

        // finder view model
        services.AddTransient<FinderViewModel>();

        // misc page view models
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<ModDetailsViewModel>();

        // details page view models
        services.AddTransient<EventDetailsViewModel>();
        services.AddTransient<TriggerDetailsViewModel>();
        services.AddTransient<EffectDetailsViewModel>();
        services.AddTransient<OptionDetailsViewModel>();

        // table page view models
        services.AddSingleton<EventTableViewModel>();
        services.AddSingleton<TriggerTableViewModel>();
        services.AddSingleton<EffectTableViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow main = ServiceProvider.GetRequiredService<MainWindow>();
        main.Show();

        IShell shell = ServiceProvider.GetRequiredService<IShell>();
        shell.GoHome();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        // dispose of the service provider
        ServiceProvider.Dispose();
    }
}

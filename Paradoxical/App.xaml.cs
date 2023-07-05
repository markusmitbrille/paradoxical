using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System.Windows;

namespace Paradoxical;

public partial class App : Application
{
    private ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new();

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

        // navigation view model
        services.AddSingleton<NavigationViewModel>();

        // page factory for navigation service
        services.AddTransient<PageFactory>(provider => pageType => (PageViewModel)provider.GetRequiredService(pageType));

        // main window and view model
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        // finder view model
        services.AddTransient<FinderViewModel>();

        // page view models
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<InfoViewModel>();

        // details page view models
        services.AddTransient<EventDetailsViewModel>();
        services.AddTransient<OptionDetailsViewModel>();
        services.AddTransient<TriggerDetailsViewModel>();
        services.AddTransient<EffectDetailsViewModel>();

        // table page view models
        services.AddSingleton<EventTableViewModel>();
        services.AddSingleton<TriggerTableViewModel>();
        services.AddSingleton<EffectTableViewModel>();

        serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow main = serviceProvider.GetRequiredService<MainWindow>();
        main.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        // dispose of the service provider
        serviceProvider.Dispose();
    }
}

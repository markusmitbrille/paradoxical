using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Services;
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

        services.AddSingleton<IBuildService, BuildService>();
        services.AddSingleton<IDataService, DataService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IMediatorService, MediatorService>();

        services.AddSingleton<IElementService, ElementService>();
        services.AddSingleton<IModService, ModService>();
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IOnActionService, OnActionService>();
        services.AddSingleton<IDecisionService, DecisionService>();
        services.AddSingleton<ITriggerService, TriggerService>();
        services.AddSingleton<IEffectService, EffectService>();
        services.AddSingleton<IOptionService, OptionService>();
        services.AddSingleton<IPortraitService, PortraitService>();

        services.AddSingleton<NavigationViewModel>();
        services.AddSingleton<PageFactory>(provider => pageType => (PageViewModelBase)provider.GetRequiredService(pageType));

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>(provider => new()
        {
            DataContext = provider.GetRequiredService<MainViewModel>(),
        });

        services.AddTransient<FindDialogViewModel>();

        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<InfoViewModel>();

        services.AddSingleton<EventDetailsViewModel>();
        services.AddSingleton<EventTableViewModel>();

        services.AddSingleton<OnActionDetailsViewModel>();
        services.AddSingleton<OnActionTableViewModel>();

        services.AddSingleton<DecisionDetailsViewModel>();
        services.AddSingleton<DecisionTableViewModel>();

        services.AddSingleton<TriggerDetailsViewModel>();
        services.AddSingleton<TriggerTableViewModel>();

        services.AddSingleton<EffectDetailsViewModel>();
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

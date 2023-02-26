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
            DataContext = provider.GetRequiredService<MainViewModel>()
        });

        services.AddSingleton<FindDialogViewModel>();
        services.AddSingleton<FindDialogView>(provider => new()
        {
            DataContext = provider.GetRequiredService<FindDialogViewModel>()
        });

        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<AboutView>(provider => new()
        {
            DataContext = provider.GetRequiredService<AboutViewModel>()
        });

        services.AddSingleton<InfoViewModel>();
        services.AddSingleton<InfoView>(provider => new()
        {
            DataContext = provider.GetRequiredService<InfoViewModel>()
        });

        services.AddSingleton<EventDetailsViewModel>();
        services.AddSingleton<EventDetailsView>(provider => new()
        {
            DataContext = provider.GetRequiredService<EventDetailsViewModel>()
        });

        services.AddSingleton<EventTableViewModel>();
        services.AddSingleton<EventTableView>(provider => new()
        {
            DataContext = provider.GetRequiredService<EventTableViewModel>()
        });

        services.AddSingleton<OnActionDetailsViewModel>();
        services.AddSingleton<OnActionDetailsView>(provider => new()
        {
            DataContext = provider.GetRequiredService<OnActionDetailsViewModel>()
        });

        services.AddSingleton<DecisionDetailsViewModel>();
        services.AddSingleton<DecisionDetailsView>(provider => new()
        {
            DataContext = provider.GetRequiredService<DecisionDetailsViewModel>()
        });

        services.AddSingleton<DecisionTableViewModel>();
        services.AddSingleton<DecisionTableView>(provider => new()
        {
            DataContext = provider.GetRequiredService<DecisionTableViewModel>()
        });

        services.AddSingleton<TriggerDetailsViewModel>();
        services.AddSingleton<TriggerDetailsView>(provider => new()
        {
            DataContext = provider.GetRequiredService<TriggerDetailsViewModel>()
        });

        services.AddSingleton<TriggerTableViewModel>();
        services.AddSingleton<TriggerTableView>(provider => new()
        {
            DataContext = provider.GetRequiredService<TriggerTableViewModel>()
        });

        services.AddSingleton<EffectDetailsViewModel>();
        services.AddSingleton<EffectDetailsView>(provider => new()
        {
            DataContext = provider.GetRequiredService<EffectDetailsViewModel>()
        });

        services.AddSingleton<EffectTableViewModel>();
        services.AddSingleton<EffectTableView>(provider => new()
        {
            DataContext = provider.GetRequiredService<EffectTableViewModel>()
        });

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

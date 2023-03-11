using Microsoft.Extensions.DependencyInjection;
using Paradoxical.Core;
using Paradoxical.Services;
using Paradoxical.Services.Components;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Relationships;
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
        services.AddSingleton<IOnActionService, OnActionService>();
        services.AddSingleton<IDecisionService, DecisionService>();
        services.AddSingleton<ITriggerService, TriggerService>();
        services.AddSingleton<IEffectService, EffectService>();

        // component services
        services.AddSingleton<IPortraitService, PortraitService>();

        // relationship services
        services.AddSingleton<IEventTriggerService, EventTriggerService>();
        services.AddSingleton<IEventImmediateService, EventImmediateService>();
        services.AddSingleton<IEventAfterService, EventAfterService>();

        services.AddSingleton<IOptionTriggerService, OptionTriggerService>();
        services.AddSingleton<IOptionEffectService, OptionEffectService>();

        services.AddSingleton<IOnActionTriggerService, OnActionTriggerService>();
        services.AddSingleton<IOnActionEffectService, OnActionEffectService>();
        services.AddSingleton<IOnActionOnActionService, OnActionOnActionService>();
        services.AddSingleton<IOnActionEventService, OnActionEventService>();

        services.AddSingleton<IDecisionValidService, DecisionValidService>();
        services.AddSingleton<IDecisionFailureService, DecisionFailureService>();
        services.AddSingleton<IDecisionShownService, DecisionShownService>();
        services.AddSingleton<IDecisionEffectService, DecisionEffectService>();

        // navigation view model
        services.AddSingleton<NavigationViewModel>();

        // page factory for navigation service
        services.AddTransient<PageFactory>(provider => pageType => (PageViewModelBase)provider.GetRequiredService(pageType));

        // main window and view model
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>(provider => new()
        {
            DataContext = provider.GetRequiredService<MainViewModel>(),
        });

        // finder view model
        services.AddTransient<FindDialogViewModel>();

        // page view models
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<InfoViewModel>();

        // details page view models
        services.AddTransient<EventDetailsViewModel>();
        services.AddTransient<OptionDetailsViewModel>();
        services.AddTransient<OnActionDetailsViewModel>();
        services.AddTransient<DecisionDetailsViewModel>();
        services.AddTransient<TriggerDetailsViewModel>();
        services.AddTransient<EffectDetailsViewModel>();

        // table page view models
        services.AddSingleton<OnActionTableViewModel>();
        services.AddSingleton<EventTableViewModel>();
        services.AddSingleton<DecisionTableViewModel>();
        services.AddSingleton<TriggerTableViewModel>();
        services.AddSingleton<EffectTableViewModel>();

        // relationship view models
        services.AddTransient<EventTriggerViewModel>();
        services.AddTransient<EventImmediateViewModel>();
        services.AddTransient<EventAfterViewModel>();

        services.AddTransient<OptionTriggerService>();
        services.AddTransient<OptionEffectService>();

        services.AddTransient<OnActionTriggerViewModel>();
        services.AddTransient<OnActionEffectViewModel>();
        services.AddTransient<OnActionOnActionViewModel>();
        services.AddTransient<OnActionEventViewModel>();

        services.AddTransient<DecisionValidViewModel>();
        services.AddTransient<DecisionFailureViewModel>();
        services.AddTransient<DecisionShownViewModel>();
        services.AddTransient<DecisionEffectViewModel>();

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

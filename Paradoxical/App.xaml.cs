using Paradoxical.Services;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System.Windows;

namespace Paradoxical;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        DataService data = new();

        ModService modService = new(data);
        EventService eventService = new(data);
        OnActionService onActionService = new(data);
        DecisionService decisionService = new(data);
        TriggerService triggerService = new(data);
        EffectService effectService = new(data);
        OptionService optionService = new(data);
        PortraitService portraitService = new(data);

        BuildService build = new(
            modService,
            eventService,
            onActionService,
            decisionService,
            triggerService,
            effectService,
            optionService,
            portraitService);

        AboutPageViewModel aboutPage = new();

        // ask chat about DI frameworks, navigation

        ApplicationViewModel app = new(data, build);

        MainViewModel vm = new(app);
        MainWindow main = new()
        {
            DataContext = vm,
        };

        main.Show();
    }
}

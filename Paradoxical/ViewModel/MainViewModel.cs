using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System.Linq;
using System.Threading.Tasks;

using Application = System.Windows.Application;

namespace Paradoxical.ViewModel;

public class MainViewModel : ObservableObject
{
    public NavigationViewModel Navigation { get; }
    public FinderViewModel Finder { get; }

    public IMediatorService Mediator { get; }
    public IFileService File { get; }

    public IEventService EventService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    public MainViewModel(
        NavigationViewModel navigation,
        FinderViewModel finder,
        IMediatorService mediator,
        IFileService file,
        IEventService eventService,
        ITriggerService triggerService,
        IEffectService effectService)
    {
        Navigation = navigation;
        Finder = finder;

        Mediator = mediator;
        File = file;

        EventService = eventService;
        TriggerService = triggerService;
        EffectService = effectService;
    }

    private RelayCommand? newCommand;
    public RelayCommand NewCommand => newCommand ??= new(New);

    private void New()
    {
        File.New();
        Navigation.GoHome();
    }

    private RelayCommand? openCommand;
    public RelayCommand OpenCommand => openCommand ??= new(Open);

    private void Open()
    {
        File.Open();
        Navigation.GoHome();
    }

    private RelayCommand? exportCommand;
    public RelayCommand ExportCommand => exportCommand ??= new(Export);

    private void Export()
    {
        File.Export();
    }

    private RelayCommand? exportAsCommand;
    public RelayCommand ExportAsCommand => exportAsCommand ??= new(ExportAs);

    private void ExportAs()
    {
        File.ExportAs();
    }

    private RelayCommand? exitCommand;
    public RelayCommand ExitCommand => exitCommand ??= new(Exit);

    private void Exit()
    {
        Mediator.Send<ShutdownMessage>(new());
        Application.Current.Shutdown();
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
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

            Navigation.Navigate<EventDetailsViewModel>();
            Mediator.Send<SelectMessage>(new(model));
        }
        if (Finder.Selected is TriggerViewModel)
        {
            Trigger model = (Trigger)Finder.Selected.Model;

            Navigation.Navigate<TriggerDetailsViewModel>();
            Mediator.Send<SelectMessage>(new(model));
        }
        if (Finder.Selected is EffectViewModel)
        {
            Effect model = (Effect)Finder.Selected.Model;

            Navigation.Navigate<EffectDetailsViewModel>();
            Mediator.Send<SelectMessage>(new(model));
        }
    }

    private RelayCommand? goToInfoPageCommand;
    public RelayCommand GoToInfoPageCommand => goToInfoPageCommand ??= new(GoToInfoPage);

    private void GoToInfoPage()
    {
        Navigation.Navigate<InfoViewModel>();
    }

    private RelayCommand? goToEventPageCommand;
    public RelayCommand GoToEventPageCommand => goToEventPageCommand ??= new(GoToEventPage);

    private void GoToEventPage()
    {
        Navigation.Navigate<EventTableViewModel>();
    }

    private RelayCommand? goToTriggerPageCommand;
    public RelayCommand GoToTriggerPageCommand => goToTriggerPageCommand ??= new(GoToTriggerPage);

    private void GoToTriggerPage()
    {
        Navigation.Navigate<TriggerTableViewModel>();
    }

    private RelayCommand? goToEffectPageCommand;
    public RelayCommand GoToEffectPageCommand => goToEffectPageCommand ??= new(GoToEffectPage);

    private void GoToEffectPage()
    {
        Navigation.Navigate<EffectTableViewModel>();
    }

    private RelayCommand? goToAboutPageCommand;
    public RelayCommand GoToAboutPageCommand => goToAboutPageCommand ??= new(GoToAboutPage);

    private void GoToAboutPage()
    {
        Navigation.Navigate<AboutViewModel>();
    }
}
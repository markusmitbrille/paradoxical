using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Paradoxical.ViewModel;

public class MainViewModel : ViewModelBase
{
    public NavigationViewModel Navigation { get; }
    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }
    public IFileService File { get; }
    public IElementService Element { get; }

    public MainViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        IFileService file,
        IElementService element)
    {
        Navigation = navigation;
        Finder = finder;
        Mediator = mediator;
        File = file;
        Element = element;
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

    private RelayCommand? goToOnActionPageCommand;
    public RelayCommand GoToOnActionPageCommand => goToOnActionPageCommand ??= new(GoToOnActionPage);

    private void GoToOnActionPage()
    {
        Navigation.Navigate<OnActionTableViewModel>();
    }

    private RelayCommand? goToDecisionPageCommand;
    public RelayCommand GoToDecisionPageCommand => goToDecisionPageCommand ??= new(GoToDecisionPage);

    private void GoToDecisionPage()
    {
        Navigation.Navigate<DecisionTableViewModel>();
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

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        var details = Navigation.CurrentPage as IElementDetailsViewModel;
        var current = details?.Selected;

        // TODO: get all elements, except current

        Finder.Items = Array.Empty<IElementViewModel>();

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        // TODO: navigate to appropriate page
        // TODO: set selected element from dlg
    }
}
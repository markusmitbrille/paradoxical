using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Messages;
using Paradoxical.Services;
using System.Windows;

namespace Paradoxical.ViewModel;

public class MainViewModel : ObservableObject
{
    public NavigationViewModel Navigation { get; }
    public FinderViewModel Finder { get; }
    public IMediatorService Mediator { get; }
    public IFileService File { get; }

    public MainViewModel(
        NavigationViewModel navigation,
        FinderViewModel finder,
        IMediatorService mediator,
        IFileService file)
    {
        Navigation = navigation;
        Finder = finder;
        Mediator = mediator;
        File = file;
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
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Services;
using System.Collections.Generic;
using System.Windows;

namespace Paradoxical.ViewModel;

public class MainViewModel : ViewModelBase
{
    public IFileService File { get; }
    public NavigationViewModel Navigation { get; }

    public MainViewModel(
        IFileService file,
        NavigationViewModel navigation,
        AboutViewModel aboutPage,
        InfoViewModel infoPage,
        EventTableViewModel eventPage,
        OnActionTableViewModel onActionPage,
        DecisionTableViewModel decisionPage,
        TriggerTableViewModel triggerPage,
        EffectTableViewModel effectPage)
    {
        File = file;
        Navigation = navigation;

        Pages = new PageViewModelBase[]
        {
            infoPage,
            eventPage,
            onActionPage,
            decisionPage,
            triggerPage,
            effectPage,
            aboutPage,
        };
    }

    public IEnumerable<PageViewModelBase> Pages { get; }

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
        Application.Current.Shutdown();
    }
}
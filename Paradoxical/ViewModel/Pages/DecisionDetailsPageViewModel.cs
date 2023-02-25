﻿using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Services;
using Paradoxical.View;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class DecisionDetailsPageViewModel : PageViewModelBase
{
    public override string PageName => "Decision Details";

    public ApplicationViewModel App { get; }
    public IDecisionService Service { get; }

    private DecisionViewModel? selected;
    public DecisionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public DecisionDetailsPageViewModel(ApplicationViewModel app, IDecisionService service)
    {
        App = app;
        Service = service;
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new AsyncRelayCommand(Find);

    private async Task Find()
    {
        var items = from model in Service.Get()
                    where model != Selected?.Model
                    select DecisionViewModel.Get(model);

        FindDialogViewModel vm = new(items)
        {
            DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
        };
        FindDialogView dlg = new()
        {
            DataContext = vm,
        };

        await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

        if (vm.DialogResult != true)
        { return; }

        if (vm.Selected == null)
        { return; }

        if (vm.Selected is not DecisionViewModel selected)
        { return; }

        Selected = selected;
    }
}
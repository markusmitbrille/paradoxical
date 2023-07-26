using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.IO;
using System.Linq;

namespace Paradoxical.ViewModel;

public class ScriptDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Script Details";

    public IModService ModService { get; }
    public IScriptService ScriptService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ScriptViewModel? selected;
    public ScriptViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public ScriptDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IModService modService,
        IScriptService scriptService)
        : base(shell, mediator)
    {
        ModService = modService;
        ScriptService = scriptService;
    }

    public override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        Mediator.Unregister<SaveMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    void IMessageHandler<ShutdownMessage>.Handle(ShutdownMessage message)
    {
        Save();
    }

    public void Load(Script model)
    {
        var selected = ScriptService.Get(model);
        Selected = new() { Model = selected };
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        ScriptService.Update(Selected.Model);
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Script model = new();
        ScriptService.Insert(model);

        var page = Shell.Navigate<ScriptDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Script model = new(Selected.Model);

        ScriptService.Insert(model);

        var page = Shell.Navigate<ScriptDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        ScriptService.Delete(Selected.Model);

        Shell.Navigate<ScriptTableViewModel>();

        var historyPages = Shell.PageHistory.OfType<ScriptDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<ScriptDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }
}

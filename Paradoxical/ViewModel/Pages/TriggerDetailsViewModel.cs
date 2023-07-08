using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Linq;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Trigger Details";

    public ITriggerService TriggerService { get; }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public TriggerDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(shell, mediator)
    {
        TriggerService = triggerService;
    }

    protected override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatingFrom()
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

    public void Load(Trigger model)
    {
        var selected = TriggerService.Get(model);
        Selected = new() { Model = selected };
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload, CanReload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }
    private bool CanReload()
    {
        return Selected != null;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save, CanSave);

    private void Save()
    {
        if (Selected == null)
        { return; }

        TriggerService.Update(Selected.Model);
    }
    public bool CanSave()
    {
        return Selected != null;
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Trigger model = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

        TriggerService.Insert(model);

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Trigger model = new(Selected.Model);

        TriggerService.Insert(model);

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanDuplicate()
    {
        return Selected != null;
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        TriggerService.Delete(Selected.Model);

        Shell.Navigate<TriggerTableViewModel>();

        var historyPages = Shell.PageHistory.OfType<TriggerDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<TriggerDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }
    private bool CanDelete()
    {
        return Selected != null;
    }
}

using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.ViewModel;

public class ScriptDetailsViewModel : PageViewModel
    , IEquatable<ScriptDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => $"Script {Selected?.Id.ToString() ?? "Details"}";

    private void RefreshPageName() => OnPropertyChanged(nameof(PageName));

    public override bool IsValid
    {
        get
        {
            if (Selected == null)
            { return false; }

            var model = ScriptService.Find(Selected.Model);

            if (model == null)
            { return false; }

            return true;
        }
    }

    public IDataService DataService { get; }
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
        IDataService dataService,
        IModService modService,
        IScriptService scriptService)
        : base(shell, mediator)
    {
        DataService = dataService;
        ModService = modService;
        ScriptService = scriptService;
    }

    public override void OnNavigatedTo()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

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
        model = ScriptService.Get(model);
        Selected = new() { Model = model };

        RefreshPageName();

        DataService.BeginTransaction();
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        var model = Selected.Model;
        Selected = null;

        Load(model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        ScriptService.Update(Selected.Model);

        RefreshPageName();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
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
    }

    #region Equality

    public override bool Equals(object? obj)
    {
        return Equals(obj as ScriptDetailsViewModel);
    }

    public bool Equals(ScriptDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<ScriptViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(ScriptDetailsViewModel? left, ScriptDetailsViewModel? right)
    {
        return EqualityComparer<ScriptDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(ScriptDetailsViewModel? left, ScriptDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}

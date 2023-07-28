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

public class TriggerDetailsViewModel : PageViewModel
    , IEquatable<TriggerDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => $"Trigger {Selected?.Id.ToString() ?? "Details"}";

    private void RefreshPageName() => OnPropertyChanged(nameof(PageName));

    public override bool IsValid
    {
        get
        {
            if (Selected == null)
            { return false; }

            var model = TriggerService.Find(Selected.Model);

            if (model == null)
            { return false; }

            return true;
        }
    }

    public IDataService DataService { get; }
    public IModService ModService { get; }
    public ITriggerService TriggerService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public string Output
    {
        get
        {
            if (Selected == null)
            { return string.Empty; }

            using StringWriter writer = new();

            Selected.Model.Write(writer, ModService);

            return writer.ToString();
        }
    }

    private void RefreshOutput() => OnPropertyChanged(nameof(Output));

    public TriggerDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IModService modService,
        ITriggerService triggerService)
        : base(shell, mediator)
    {
        DataService = dataService;
        ModService = modService;
        TriggerService = triggerService;
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

    public void Load(Trigger model)
    {
        model = TriggerService.Get(model);
        Selected = new() { Model = model };

        RefreshPageName();
        RefreshOutput();

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

        Load(Selected.Model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        TriggerService.Update(Selected.Model);

        RefreshPageName();
        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Trigger model = new();
        TriggerService.Insert(model);

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Trigger model = new(Selected.Model);

        TriggerService.Insert(model);

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        TriggerService.Delete(Selected.Model);

        Shell.Navigate<TriggerTableViewModel>();
        Shell.InvalidatePage(this);
    }

    #region Equality

    public override bool Equals(object? obj)
    {
        return Equals(obj as TriggerDetailsViewModel);
    }

    public bool Equals(TriggerDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<TriggerViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(TriggerDetailsViewModel? left, TriggerDetailsViewModel? right)
    {
        return EqualityComparer<TriggerDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(TriggerDetailsViewModel? left, TriggerDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}

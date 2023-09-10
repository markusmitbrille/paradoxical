using CommunityToolkit.Mvvm.ComponentModel;
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

public class TriggerDetailsViewModel : DetailsViewModel
    , IEquatable<TriggerDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
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

        TriggerService.Update(Selected.Model);

        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand? refreshOutputCommand;
    public RelayCommand RefreshOutputCommand => refreshOutputCommand ??= new(RefreshOutput);

    private void RefreshOutput()
    {
        if (Selected == null)
        { return; }

        OnPropertyChanged(nameof(Output));
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

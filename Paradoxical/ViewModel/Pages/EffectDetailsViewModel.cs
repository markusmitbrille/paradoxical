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

public class EffectDetailsViewModel : PageViewModel
    , IEquatable<EffectDetailsViewModel?>
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => $"Effect {Selected?.Id.ToString() ?? "Details"}";

    private void RefreshPageName() => OnPropertyChanged(nameof(PageName));

    public override bool IsValid
    {
        get
        {
            if (Selected == null)
            { return false; }

            var model = EffectService.Find(Selected.Model);

            if (model == null)
            { return false; }

            return true;
        }
    }

    public IDataService DataService { get; }
    public IModService ModService { get; }
    public IEffectService EffectService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private EffectViewModel? selected;
    public EffectViewModel? Selected
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

    public EffectDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IModService modService,
        IEffectService effectService)
        : base(shell, mediator)
    {
        DataService = dataService;
        ModService = modService;
        EffectService = effectService;
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

    public void Load(Effect model)
    {
        model = EffectService.Get(model);
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

        EffectService.Update(Selected.Model);

        RefreshPageName();
        RefreshOutput();

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Effect model = new();
        EffectService.Insert(model);

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Effect model = new(Selected.Model);

        EffectService.Insert(model);

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand? deleteCommand;
    public RelayCommand DeleteCommand => deleteCommand ??= new(Delete);

    private void Delete()
    {
        if (Selected == null)
        { return; }

        EffectService.Delete(Selected.Model);

        Shell.Navigate<EffectTableViewModel>();
        Shell.InvalidatePage(this);
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
        return Equals(obj as EffectDetailsViewModel);
    }

    public bool Equals(EffectDetailsViewModel? other)
    {
        return other is not null &&
               EqualityComparer<EffectViewModel?>.Default.Equals(selected, other.selected);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(selected);
    }

    public static bool operator ==(EffectDetailsViewModel? left, EffectDetailsViewModel? right)
    {
        return EqualityComparer<EffectDetailsViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(EffectDetailsViewModel? left, EffectDetailsViewModel? right)
    {
        return !(left == right);
    }

    #endregion
}

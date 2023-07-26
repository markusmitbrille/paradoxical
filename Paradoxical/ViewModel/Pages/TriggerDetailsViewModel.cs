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
    public override string PageName => "Trigger Details";

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

    public TriggerDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IModService modService,
        ITriggerService triggerService)
        : base(shell, mediator)
    {
        ModService = modService;
        TriggerService = triggerService;
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

    public void Load(Trigger model)
    {
        var selected = TriggerService.Get(model);
        Selected = new() { Model = selected };

        LoadRaw();
    }

    private void LoadRaw()
    {
        if (Selected == null)
        { return; }

        if (Selected.Raw == null)
        {
            OverrideRaw = false;

            // regenerate view model raw
            Raw = GenerateRaw();
        }
        else
        {
            OverrideRaw = true;

            // set view model raw to model and wrapper raw
            Raw = Selected.Raw;
        }
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

        SaveRaw();

        TriggerService.Update(Selected.Model);
    }

    private void SaveRaw()
    {
        if (Selected == null)
        { return; }

        if (OverrideRaw == true)
        {
            // overwrite model raw
            Selected.Raw = Raw;
        }
        else
        {
            // regenerate view model raw
            Raw = GenerateRaw();

            // clear model and wrapper raw
            Selected.Raw = null;
        }
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

        var historyPages = Shell.PageHistory.OfType<TriggerDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<TriggerDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }

    #region Raw

    private bool? overrideRaw = null;
    public bool? OverrideRaw
    {
        get => overrideRaw;
        set => SetProperty(ref overrideRaw, value);
    }

    private string raw = string.Empty;
    public string Raw
    {
        get => raw;
        set => SetProperty(ref raw, value);
    }

    private RelayCommand<bool?>? toggleOverrideRawCommand;
    public RelayCommand<bool?> ToggleOverrideRawCommand => toggleOverrideRawCommand ??= new(ToggleOverrideRaw);

    private void ToggleOverrideRaw(bool? isChecked)
    {
        if (isChecked == true)
        {
            ToggleOverrideRawOn();
        }
        if (isChecked == false)
        {
            ToggleOverrideRawOff();
        }
    }

    private void ToggleOverrideRawOn()
    {
        if (Selected == null)
        { return; }

        Raw = GenerateRaw();
        Selected.Raw = Raw;
    }

    private void ToggleOverrideRawOff()
    {
        if (Selected == null)
        { return; }

        Raw = GenerateRaw();
        Selected.Raw = null;
    }

    private string GenerateRaw()
    {
        if (Selected == null)
        { return string.Empty; }

        using StringWriter writer = new();

        Selected.Model.Write(writer, ModService);

        return writer.ToString();
    }

    #endregion

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

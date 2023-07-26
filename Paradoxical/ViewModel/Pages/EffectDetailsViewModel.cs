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
    public override string PageName => "Effect Details";

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

    public EffectDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IModService modService,
        IEffectService effectService)
        : base(shell, mediator)
    {
        ModService = modService;
        EffectService = effectService;
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

    public void Load(Effect model)
    {
        var selected = EffectService.Get(model);
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

        EffectService.Update(Selected.Model);
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

        var historyPages = Shell.PageHistory.OfType<EffectDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<EffectDetailsViewModel>()
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

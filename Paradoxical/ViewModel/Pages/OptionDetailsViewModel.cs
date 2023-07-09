using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class OptionDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Option Details";

    public FinderViewModel Finder { get; }

    public IOptionService OptionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    private OptionViewModel? selected;
    public OptionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private ObservableCollection<TriggerViewModel>? triggers;
    public ObservableCollection<TriggerViewModel> Triggers => triggers ??= new();

    private ObservableCollection<EffectViewModel>? effects;
    public ObservableCollection<EffectViewModel> Effects => effects ??= new();

    public OptionDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        FinderViewModel finder,
        IOptionService optionService,
        ITriggerService triggerService,
        IEffectService effectService)
        : base(shell, mediator)
    {
        Finder = finder;

        OptionService = optionService;
        TriggerService = triggerService;
        EffectService = effectService;
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

    public void Load(Option model)
    {
        var selected = OptionService.Get(model);

        var triggers = OptionService.GetTriggers(selected)
            .Select(model => new TriggerViewModel() { Model = model });

        var effects = OptionService.GetEffects(selected)
            .Select(model => new EffectViewModel() { Model = model });

        Selected = new() { Model = selected };

        Triggers.Clear();
        Triggers.AddRange(triggers);

        Effects.Clear();
        Effects.AddRange(effects);
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

        OptionService.Update(Selected.Model);
    }
    private bool CanSave()
    {
        return Selected != null;
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create, CanCreate);

    private void Create()
    {
        if (Selected == null)
        { return; }

        Option model = new() { EventId = Selected.EventId };

        OptionService.Insert(model);

        var page = Shell.Navigate<OptionDetailsViewModel>();
        page.Load(model);
    }
    private bool CanCreate()
    {
        return Selected != null;
    }

    private RelayCommand? duplicateCommand;
    public RelayCommand DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate()
    {
        if (Selected == null)
        { return; }

        Option model = new(Selected.Model);
        OptionService.Insert(model);

        var triggers = OptionService.GetTriggers(Selected.Model);
        foreach (var trigger in triggers)
        {
            OptionService.AddTrigger(model, trigger);
        }

        var effects = OptionService.GetEffects(Selected.Model);
        foreach (var effect in effects)
        {
            OptionService.AddEffect(model, effect);
        }

        var page = Shell.Navigate<OptionDetailsViewModel>();
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

        Event owner = OptionService.GetEvent(Selected.Model);
        
        OptionService.Delete(Selected.Model);

        var page = Shell.Navigate<EventDetailsViewModel>();
        page.Load(owner);

        var historyPages = Shell.PageHistory.OfType<OptionDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        var futurePages = Shell.PageFuture.OfType<OptionDetailsViewModel>()
            .Where(page => page.Selected?.Model == Selected.Model)
            .ToArray();

        Shell.PageHistory.RemoveAll(page => historyPages.Contains(page));
        Shell.PageFuture.RemoveAll(page => futurePages.Contains(page));
    }
    private bool CanDelete()
    {
        return Selected != null;
    }

    private RelayCommand? goToPreviousOptionCommand;
    public RelayCommand GoToPreviousOptionCommand => goToPreviousOptionCommand ??= new(GoToPreviousOption, CanGoToPreviousOption);

    private void GoToPreviousOption()
    {
        if (Selected == null)
        { return; }

        throw new NotImplementedException();
    }
    private bool CanGoToPreviousOption()
    {
        return Selected != null;
    }

    private RelayCommand? goToNextOptionCommand;
    public RelayCommand GoToNextOptionCommand => goToNextOptionCommand ??= new(GoToNextOption, CanGoToNextOption);

    private void GoToNextOption()
    {
        if (Selected == null)
        { return; }

        throw new NotImplementedException();
    }
    private bool CanGoToNextOption()
    {
        return Selected != null;
    }

    #region Trigger Commands

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger, CanCreateTrigger);

    private void CreateTrigger()
    {
        if (Selected == null)
        { return; }

        Option owner = Selected.Model;
        Trigger relation = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

        TriggerService.Insert(relation);
        OptionService.AddTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        Triggers.Add(observable);
    }
    private bool CanCreateTrigger()
    {
        return Selected != null;
    }

    private AsyncRelayCommand? addTriggerCommand;
    public AsyncRelayCommand AddTriggerCommand => addTriggerCommand ??= new(AddTrigger);

    private async Task AddTrigger()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = TriggerService.Get()
            .Select(model => new TriggerViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = Selected.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        OptionService.AddTrigger(owner, relation);

        TriggerViewModel observable = new() { Model = relation };
        Triggers.Add(observable);
    }

    private RelayCommand<TriggerViewModel>? removeTriggerCommand;
    public RelayCommand<TriggerViewModel> RemoveTriggerCommand => removeTriggerCommand ??= new(RemoveTrigger, CanRemoveTrigger);

    private void RemoveTrigger(TriggerViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        OptionService.RemoveTrigger(Selected.Model, observable.Model);
        Triggers.Remove(observable);
    }
    private bool CanRemoveTrigger(TriggerViewModel? observable)
    {
        return Selected != null && observable != null;
    }

    private RelayCommand<TriggerViewModel>? goToTriggerCommand;
    public RelayCommand<TriggerViewModel> GoToTriggerCommand => goToTriggerCommand ??= new(GoToTrigger, CanGoToTrigger);

    private void GoToTrigger(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Trigger model = observable.Model;

        var page = Shell.Navigate<TriggerDetailsViewModel>();
        page.Load(model);
    }
    private bool CanGoToTrigger(TriggerViewModel? observable)
    {
        return observable != null;
    }

    #endregion

    #region Effect Commands

    private RelayCommand? createEffectCommand;
    public RelayCommand CreateEffectCommand => createEffectCommand ??= new(CreateEffect, CanCreateEffect);

    private void CreateEffect()
    {
        if (Selected == null)
        { return; }

        Option owner = Selected.Model;
        Effect relation = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };

        EffectService.Insert(relation);
        OptionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }
    private bool CanCreateEffect()
    {
        return Selected != null;
    }

    private AsyncRelayCommand? addEffectCommand;
    public AsyncRelayCommand AddEffectCommand => addEffectCommand ??= new(AddEffect);

    private async Task AddEffect()
    {
        if (Selected == null)
        { return; }

        Save();

        Finder.Items = EffectService.Get()
            .Select(model => new EffectViewModel() { Model = model });

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = Selected.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        OptionService.AddEffect(owner, relation);

        EffectViewModel observable = new() { Model = relation };
        Effects.Add(observable);
    }

    private RelayCommand<EffectViewModel>? removeEffectCommand;
    public RelayCommand<EffectViewModel> RemoveEffectCommand => removeEffectCommand ??= new(RemoveEffect, CanRemoveEffect);

    private void RemoveEffect(EffectViewModel? observable)
    {
        if (Selected == null)
        { return; }

        if (observable == null)
        { return; }

        OptionService.RemoveEffect(Selected.Model, observable.Model);
        Effects.Remove(observable);
    }
    private bool CanRemoveEffect(EffectViewModel? observable)
    {
        return Selected != null && observable != null;
    }

    private RelayCommand<EffectViewModel>? goToEffectCommand;
    public RelayCommand<EffectViewModel> GoToEffectCommand => goToEffectCommand ??= new(GoToEffect, CanGoToEffect);

    private void GoToEffect(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = observable.Model;

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanGoToEffect(EffectViewModel? observable)
    {
        return observable != null;
    }

    #endregion
}
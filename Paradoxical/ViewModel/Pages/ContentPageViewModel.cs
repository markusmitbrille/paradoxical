using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Paradoxical.ViewModel;

public class ContentPageViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Content";

    public IDataService DataService { get; }

    public IModService ModService { get; }
    public IScriptService ScriptService { get; }
    public IEventService EventService { get; }
    public IOptionService OptionService { get; }
    public IPortraitService PortraitService { get; }
    public IDecisionService DecisionService { get; }
    public IOnionService OnionService { get; }
    public ITriggerService TriggerService { get; }
    public IEffectService EffectService { get; }

    public IFinder Finder { get; }

    private ModViewModel ModViewModel { get; set; } = new();

    private ModelMap<Script, ScriptViewModel> ScriptModelMap { get; set; } = new();
    private ModelMap<Event, EventViewModel> EventModelMap { get; set; } = new();
    private ModelMap<Option, OptionViewModel> OptionModelMap { get; set; } = new();
    private ModelMap<Onion, OnionViewModel> OnionModelMap { get; set; } = new();
    private ModelMap<Portrait, PortraitViewModel> PortraitModelMap { get; set; } = new();
    private ModelMap<Decision, DecisionViewModel> DecisionModelMap { get; set; } = new();
    private ModelMap<Trigger, TriggerViewModel> TriggerModelMap { get; set; } = new();
    private ModelMap<Effect, EffectViewModel> EffectModelMap { get; set; } = new();

    private CollectionNode rootNode = new();
    public CollectionNode RootNode
    {
        get => rootNode;
        set => SetProperty(ref rootNode, value);
    }

    private ModBranch ModNode { get; set; } = new();

    private ObservableObject? selected;
    public ObservableObject? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public ContentPageViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IModService modService,
        IScriptService scriptService,
        IEventService eventService,
        IOptionService optionService,
        IOnionService onionService,
        IPortraitService portraitService,
        IDecisionService decisionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IFinder finder)
        : base(shell, mediator)
    {
        DataService = dataService;

        ModService = modService;
        ScriptService = scriptService;
        EventService = eventService;
        OptionService = optionService;
        OnionService = onionService;
        PortraitService = portraitService;
        DecisionService = decisionService;
        TriggerService = triggerService;
        EffectService = effectService;

        Finder = finder;
    }

    public override void OnNavigatedTo()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Load();

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

    private RelayCommand? loadCommand;
    public RelayCommand LoadCommand => loadCommand ??= new(Load);

    private void Load()
    {
        var mod = ModService.Get().SingleOrDefault();
        if (mod == null)
        {
            mod = new();
            ModService.Insert(mod);
        }

        ModViewModel = new() { Model = mod };

        ScriptModelMap = new(ScriptService.Get());
        EventModelMap = new(EventService.Get());
        OptionModelMap = new(OptionService.Get());
        OnionModelMap = new(OnionService.Get());
        PortraitModelMap = new(PortraitService.Get());
        DecisionModelMap = new(DecisionService.Get());
        TriggerModelMap = new(TriggerService.Get());
        EffectModelMap = new(EffectService.Get());

        ModNode = new() { Observable = ModViewModel };

        InitModNode(ModNode);
        InitModBranch(ModNode);

        RootNode = new();
        RootNode.Add(ModNode);

        DataService.BeginTransaction();
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (DataService.IsInTransaction)
        {
            DataService.RollbackTransaction();
        }

        Load();
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        ModService.Update(ModViewModel.Model);

        ScriptService.UpdateAll(ScriptModelMap.Models);
        EventService.UpdateAll(EventModelMap.Models);
        OptionService.UpdateAll(OptionModelMap.Models);
        OnionService.UpdateAll(OnionModelMap.Models);
        PortraitService.UpdateAll(PortraitModelMap.Models);
        DecisionService.UpdateAll(DecisionModelMap.Models);
        TriggerService.UpdateAll(TriggerModelMap.Models);
        EffectService.UpdateAll(EffectModelMap.Models);

        DataService.CommitTransaction();
        DataService.BeginTransaction();
    }

    private RelayCommand<object>? editCommand;
    public RelayCommand<object> EditCommand => editCommand ??= new(Edit, CanEdit);

    private void Edit(object? param)
    {
        if (param is not ObservableObject observable)
        { return; }

        Selected = observable;
    }
    private bool CanEdit(object? param)
    {
        return param is ObservableObject;
    }

    private RelayCommand<object>? editNodeCommand;
    public RelayCommand<object> EditNodeCommand => editNodeCommand ??= new(EditNode, CanEditNode);

    private void EditNode(object? param)
    {
        if (param is not Node node)
        { return; }

        if (param is not IObservableWrapper wrapper)
        { return; }

        var observable = wrapper.Observable;
        Selected = observable;
    }
    private bool CanEditNode(object? param)
    {
        return param is Node && param is IObservableWrapper;
    }


    #region Node Init

    private void InitModNode(ModNode node)
    {
        node.EditCommand = EditCommand;

        node.CreateScriptCommand = CreateScriptCommand;
        node.CreateEventCommand = CreateEventCommand;
        node.CreateDecisionCommand = CreateDecisionCommand;
        node.CreateTriggerCommand = CreateTriggerCommand;
        node.CreateEffectCommand = CreateEffectCommand;
    }

    private void InitModBranch(ModBranch node)
    {
        foreach (var observable in ScriptModelMap.Wrappers)
        {
            ScriptBranch child = new() { Observable = observable };

            InitScriptNode(child);
            InitScriptBranch(child);

            node.ScriptNodes.Add(child);
        }

        foreach (var observable in EventModelMap.Wrappers)
        {
            EventBranch child = new() { Observable = observable };

            InitEventNode(child);
            InitEventBranch(child);

            node.EventNodes.Add(child);
        }

        foreach (var observable in DecisionModelMap.Wrappers)
        {
            DecisionBranch child = new() { Observable = observable };

            InitDecisionNode(child);
            InitDecisionBranch(child);

            node.DecisionNodes.Add(child);
        }

        foreach (var observable in TriggerModelMap.Wrappers)
        {
            TriggerBranch child = new() { Observable = observable };

            InitTriggerNode(child);
            InitTriggerBranch(child);

            node.TriggerNodes.Add(child);
        }

        foreach (var observable in EffectModelMap.Wrappers)
        {
            EffectBranch child = new() { Observable = observable };

            InitEffectNode(child);
            InitEffectBranch(child);

            node.EffectNodes.Add(child);
        }
    }

    private void InitScriptNode(ScriptNode node)
    {
        node.DeleteCommand = DeleteScriptCommand;
        node.EditCommand = EditCommand;
    }

    private void InitScriptBranch(ScriptBranch node)
    {
    }

    private void InitEventNode(EventNode node)
    {
        node.DeleteCommand = DeleteEventCommand;
        node.EditCommand = EditCommand;

        node.CreatePortraitCommand = CreateEventPortraitCommand;
        node.CreateOptionCommand = CreateEventOptionCommand;
        node.CreateOnionCommand = CreateEventOnionCommand;

        node.AddTriggerCommand = AddEventTriggerCommand;
        node.RemoveTriggerCommand = RemoveEventTriggerCommand;

        node.AddImmediateEffectCommand = AddEventImmediateEffectCommand;
        node.RemoveImmediateEffectCommand = RemoveEventImmediateEffectCommand;

        node.AddAfterEffectCommand = AddEventAfterEffectCommand;
        node.RemoveAfterEffectCommand = RemoveEventAfterEffectCommand;
    }

    private void InitEventBranch(EventBranch node)
    {
        Event model = node.Observable.Model;

        foreach (var relation in EventService.GetPortraits(model))
        {
            var observable = PortraitModelMap[relation];
            PortraitBranch child = new() { Observable = observable };

            InitPortraitNode(child);
            InitPortraitBranch(child);

            node.PortraitNodes.Add(child);
        }

        foreach (var relation in EventService.GetOptions(model))
        {
            var observable = OptionModelMap[relation];
            OptionBranch child = new() { Observable = observable };

            InitOptionNode(child);
            InitOptionBranch(child);

            node.OptionNodes.Add(child);
        }

        foreach (var relation in EventService.GetOnions(model))
        {
            var observable = OnionModelMap[relation];
            OnionBranch child = new() { Observable = observable };

            InitOnionNode(child);
            InitOnionBranch(child);

            node.OnionNodes.Add(child);
        }

        foreach (var relation in EventService.GetTriggers(model))
        {
            var observable = TriggerModelMap[relation];
            TriggerLeaf child = new() { Observable = observable };

            InitTriggerNode(child);

            node.TriggerNodes.Add(child);
        }

        foreach (var relation in EventService.GetImmediateEffects(model))
        {
            var observable = EffectModelMap[relation];
            EffectLeaf child = new() { Observable = observable };

            InitEffectNode(child);

            node.ImmediateEffectNodes.Add(child);
        }

        foreach (var relation in EventService.GetAfterEffects(model))
        {
            var observable = EffectModelMap[relation];
            EffectLeaf child = new() { Observable = observable };

            InitEffectNode(child);

            node.AfterEffectNodes.Add(child);
        }
    }

    private void InitPortraitNode(PortraitNode node)
    {
        node.DeleteCommand = DeletePortraitCommand;
        node.EditCommand = EditCommand;
    }

    private void InitPortraitBranch(PortraitBranch node)
    {
    }

    private void InitOptionNode(OptionNode node)
    {
        node.DeleteCommand = DeleteOptionCommand;
        node.EditCommand = EditCommand;

        node.AddTriggerCommand = AddOptionTriggerCommand;
        node.RemoveTriggerCommand = RemoveOptionTriggerCommand;

        node.AddEffectCommand = AddOptionEffectCommand;
        node.RemoveEffectCommand = RemoveOptionEffectCommand;
    }

    private void InitOptionBranch(OptionBranch node)
    {
        Option model = node.Observable.Model;

        foreach (var relation in OptionService.GetTriggers(model))
        {
            var observable = TriggerModelMap[relation];
            TriggerLeaf child = new() { Observable = observable };

            InitTriggerNode(child);

            node.TriggerNodes.Add(child);
        }

        foreach (var relation in OptionService.GetEffects(model))
        {
            var observable = EffectModelMap[relation];
            EffectLeaf child = new() { Observable = observable };

            InitEffectNode(child);

            node.EffectNodes.Add(child);
        }
    }

    private void InitOnionNode(OnionNode node)
    {
        node.DeleteCommand = DeleteOnionCommand;
        node.EditCommand = EditCommand;
    }

    private void InitOnionBranch(OnionBranch node)
    {
    }

    private void InitDecisionNode(DecisionNode node)
    {
        node.DeleteCommand = DeleteDecisionCommand;
        node.EditCommand = EditCommand;

        node.AddShownTriggerCommand = AddDecisionShownTriggerCommand;
        node.RemoveShownTriggerCommand = RemoveDecisionShownTriggerCommand;

        node.AddFailureTriggerCommand = AddDecisionFailureTriggerCommand;
        node.RemoveFailureTriggerCommand = RemoveDecisionFailureTriggerCommand;

        node.AddValidTriggerCommand = AddDecisionValidTriggerCommand;
        node.RemoveValidTriggerCommand = RemoveDecisionValidTriggerCommand;

        node.AddEffectCommand = AddDecisionEffectCommand;
        node.RemoveEffectCommand = RemoveDecisionEffectCommand;
    }

    private void InitDecisionBranch(DecisionBranch node)
    {
        Decision model = node.Observable.Model;

        foreach (var relation in DecisionService.GetShownTriggers(model))
        {
            var observable = TriggerModelMap[relation];
            TriggerLeaf child = new() { Observable = observable };

            InitTriggerNode(child);

            node.ShownTriggerNodes.Add(child);
        }

        foreach (var relation in DecisionService.GetFailureTriggers(model))
        {
            var observable = TriggerModelMap[relation];
            TriggerLeaf child = new() { Observable = observable };

            InitTriggerNode(child);

            node.FailureTriggerNodes.Add(child);
        }

        foreach (var relation in DecisionService.GetValidTriggers(model))
        {
            var observable = TriggerModelMap[relation];
            TriggerLeaf child = new() { Observable = observable };

            InitTriggerNode(child);

            node.ValidTriggerNodes.Add(child);
        }

        foreach (var relation in DecisionService.GetEffects(model))
        {
            var observable = EffectModelMap[relation];
            EffectLeaf child = new() { Observable = observable };

            InitEffectNode(child);

            node.ValidTriggerNodes.Add(child);
        }
    }

    private void InitTriggerNode(TriggerNode node)
    {
        node.DeleteCommand = DeleteTriggerCommand;
        node.EditCommand = EditCommand;
    }

    private void InitTriggerBranch(TriggerBranch node)
    {
    }

    private void InitEffectNode(EffectNode node)
    {
        node.DeleteCommand = DeleteEffectCommand;
        node.EditCommand = EditCommand;
    }

    private void InitEffectBranch(EffectBranch node)
    {
    }

    #endregion


    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        var model = new Script();
        ScriptService.Insert(model);

        //Setup();

        var observable = ScriptModelMap[model];
        Selected = observable;
    }

    private RelayCommand<object>? deleteScriptCommand;
    public RelayCommand<object> DeleteScriptCommand => deleteScriptCommand ??= new(DeleteScript, CanDeleteScript);

    private void DeleteScript(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        var model = observable.Model;
        ScriptService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteScript(object? param)
    {
        return param is ScriptViewModel;
    }

    #endregion


    #region Event Commands

    private RelayCommand? createEventCommand;
    public RelayCommand CreateEventCommand => createEventCommand ??= new(CreateEvent);

    private void CreateEvent()
    {
        var model = new Event();
        EventService.Insert(model);

        //Setup();

        var observable = EventModelMap[model];
        Selected = observable;
    }

    private RelayCommand<object>? deleteEventCommand;
    public RelayCommand<object> DeleteEventCommand => deleteEventCommand ??= new(DeleteEvent, CanDeleteEvent);

    private void DeleteEvent(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var model = observable.Model;
        EventService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteEvent(object? param)
    {
        return param is EventViewModel;
    }

    private RelayCommand<object>? createEventOptionCommand;
    public RelayCommand<object> CreateEventOptionCommand => createEventOptionCommand ??= new(CreateEventOption, CanCreateEventOption);

    private void CreateEventOption(object? param)
    {
        if (param is not EventViewModel evt)
        { return; }

        Option model = new() { EventId = evt.Id };
        OptionService.Insert(model);

        //Setup();

        var observable = OptionModelMap[model];
        Selected = observable;
    }
    private bool CanCreateEventOption(object? param)
    {
        return param is EventViewModel;
    }

    private RelayCommand<object>? createEventOnionCommand;
    public RelayCommand<object> CreateEventOnionCommand => createEventOnionCommand ??= new(CreateEventOnion, CanCreateEventOnion);

    private void CreateEventOnion(object? param)
    {
        if (param is not EventViewModel evt)
        { return; }

        Onion model = new() { EventId = evt.Id };
        OnionService.Insert(model);

        //Setup();

        var observable = OnionModelMap[model];
        Selected = observable;
    }
    private bool CanCreateEventOnion(object? param)
    {
        return param is EventViewModel;
    }

    private RelayCommand<object>? createEventPortraitCommand;
    public RelayCommand<object> CreateEventPortraitCommand => createEventPortraitCommand ??= new(CreateEventPortrait, CanCreateEventPortrait);

    private void CreateEventPortrait(object? param)
    {
        if (param is not EventViewModel evt)
        { return; }

        Portrait model = new() { EventId = evt.Id };
        PortraitService.Insert(model);

        //Setup();

        var observable = PortraitModelMap[model];
        Selected = observable;
    }
    private bool CanCreateEventPortrait(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? addEventTriggerCommand;
    public AsyncRelayCommand<object> AddEventTriggerCommand => addEventTriggerCommand ??= new(AddEventTrigger, CanAddEventTrigger);

    private async Task AddEventTrigger(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var all = TriggerService.Get();
        var existing = EventService.GetTriggers(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        EventService.AddTrigger(owner, relation);

        //Setup();

        Selected = TriggerModelMap[relation];
    }
    private bool CanAddEventTrigger(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? removeEventTriggerCommand;
    public AsyncRelayCommand<object> RemoveEventTriggerCommand => removeEventTriggerCommand ??= new(RemoveEventTrigger, CanRemoveEventTrigger);

    private async Task RemoveEventTrigger(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var existing = EventService.GetTriggers(observable.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        EventService.RemoveTrigger(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveEventTrigger(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? addEventImmediateEffectCommand;
    public AsyncRelayCommand<object> AddEventImmediateEffectCommand => addEventImmediateEffectCommand ??= new(AddEventImmediateEffect, CanAddEventImmediateEffect);

    private async Task AddEventImmediateEffect(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var all = EffectService.Get();
        var existing = EventService.GetImmediateEffects(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.AddImmediateEffect(owner, relation);

        //Setup();

        Selected = EffectModelMap[relation];
    }
    private bool CanAddEventImmediateEffect(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? removeEventImmediateEffectCommand;
    public AsyncRelayCommand<object> RemoveEventImmediateEffectCommand => removeEventImmediateEffectCommand ??= new(RemoveEventImmediateEffect, CanRemoveEventImmediateEffect);

    private async Task RemoveEventImmediateEffect(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var existing = EventService.GetImmediateEffects(observable.Model);

        Finder.Items = existing.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.RemoveImmediateEffect(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveEventImmediateEffect(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? addEventAfterEffectCommand;
    public AsyncRelayCommand<object> AddEventAfterEffectCommand => addEventAfterEffectCommand ??= new(AddEventAfterEffect, CanAddEventAfterEffect);

    private async Task AddEventAfterEffect(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var all = EffectService.Get();
        var existing = EventService.GetAfterEffects(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.AddAfterEffect(owner, relation);

        //Setup();

        Selected = EffectModelMap[relation];
    }
    private bool CanAddEventAfterEffect(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? removeEventAfterEffectCommand;
    public AsyncRelayCommand<object> RemoveEventAfterEffectCommand => removeEventAfterEffectCommand ??= new(RemoveEventAfterEffect, CanRemoveEventAfterEffect);

    private async Task RemoveEventAfterEffect(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        var existing = EventService.GetAfterEffects(observable.Model);

        Finder.Items = existing.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        EventService.RemoveAfterEffect(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveEventAfterEffect(object? param)
    {
        return param is EventViewModel;
    }

    #endregion


    #region Portrait Commands

    private RelayCommand<object>? deletePortraitCommand;
    public RelayCommand<object> DeletePortraitCommand => deletePortraitCommand ??= new(DeletePortrait, CanDeletePortrait);

    private void DeletePortrait(object? param)
    {
        if (param is not PortraitViewModel observable)
        { return; }

        var model = observable.Model;
        PortraitService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeletePortrait(object? param)
    {
        return param is PortraitViewModel;
    }

    #endregion


    #region Option Commands

    private RelayCommand<object>? deleteOptionCommand;
    public RelayCommand<object> DeleteOptionCommand => deleteOptionCommand ??= new(DeleteOption, CanDeleteOption);

    private void DeleteOption(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        var model = observable.Model;
        OptionService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteOption(object? param)
    {
        return param is OptionViewModel;
    }

    private AsyncRelayCommand<object>? addOptionTriggerCommand;
    public AsyncRelayCommand<object> AddOptionTriggerCommand => addOptionTriggerCommand ??= new(AddOptionTrigger, CanAddOptionTrigger);

    private async Task AddOptionTrigger(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        var all = TriggerService.Get();
        var existing = OptionService.GetTriggers(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        OptionService.AddTrigger(owner, relation);

        //Setup();

        Selected = TriggerModelMap[relation];
    }
    private bool CanAddOptionTrigger(object? param)
    {
        return param is OptionViewModel;
    }

    private AsyncRelayCommand<object>? removeOptionTriggerCommand;
    public AsyncRelayCommand<object> RemoveOptionTriggerCommand => removeOptionTriggerCommand ??= new(RemoveOptionTrigger, CanRemoveOptionTrigger);

    private async Task RemoveOptionTrigger(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        var existing = OptionService.GetTriggers(observable.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        OptionService.RemoveTrigger(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveOptionTrigger(object? param)
    {
        return param is OptionViewModel;
    }

    private AsyncRelayCommand<object>? addOptionEffectCommand;
    public AsyncRelayCommand<object> AddOptionEffectCommand => addOptionEffectCommand ??= new(AddOptionEffect, CanAddOptionEffect);

    private async Task AddOptionEffect(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        var all = EffectService.Get();
        var existing = OptionService.GetEffects(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        OptionService.AddEffect(owner, relation);

        //Setup();

        Selected = EffectModelMap[relation];
    }
    private bool CanAddOptionEffect(object? param)
    {
        return param is OptionViewModel;
    }

    private AsyncRelayCommand<object>? removeOptionEffectCommand;
    public AsyncRelayCommand<object> RemoveOptionEffectCommand => removeOptionEffectCommand ??= new(RemoveOptionEffect, CanRemoveOptionEffect);

    private async Task RemoveOptionEffect(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        var existing = OptionService.GetEffects(observable.Model);

        Finder.Items = existing.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Option owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        OptionService.RemoveEffect(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveOptionEffect(object? param)
    {
        return param is OptionViewModel;
    }

    #endregion


    #region Onion Commands

    private RelayCommand<object>? deleteOnionCommand;
    public RelayCommand<object> DeleteOnionCommand => deleteOnionCommand ??= new(DeleteOnion, CanDeleteOnion);

    private void DeleteOnion(object? param)
    {
        if (param is not OnionViewModel observable)
        { return; }

        var model = observable.Model;
        OnionService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteOnion(object? param)
    {
        return param is OnionViewModel;
    }

    #endregion


    #region Decision Commands

    private RelayCommand? createDecisionCommand;
    public RelayCommand CreateDecisionCommand => createDecisionCommand ??= new(CreateDecision);

    private void CreateDecision()
    {
        var model = new Decision();
        DecisionService.Insert(model);

        //Setup();

        var observable = DecisionModelMap[model];
        Selected = observable;
    }

    private RelayCommand<object>? deleteDecisionCommand;
    public RelayCommand<object> DeleteDecisionCommand => deleteDecisionCommand ??= new(DeleteDecision, CanDeleteDecision);

    private void DeleteDecision(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var model = observable.Model;
        DecisionService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteDecision(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? addDecisionShownTriggerCommand;
    public AsyncRelayCommand<object> AddDecisionShownTriggerCommand => addDecisionShownTriggerCommand ??= new(AddDecisionShownTrigger, CanAddDecisionShownTrigger);

    private async Task AddDecisionShownTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var all = TriggerService.Get();
        var existing = DecisionService.GetShownTriggers(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddShownTrigger(owner, relation);

        //Setup();

        Selected = TriggerModelMap[relation];
    }
    private bool CanAddDecisionShownTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? removeDecisionShownTriggerCommand;
    public AsyncRelayCommand<object> RemoveDecisionShownTriggerCommand => removeDecisionShownTriggerCommand ??= new(RemoveDecisionShownTrigger, CanRemoveDecisionShownTrigger);

    private async Task RemoveDecisionShownTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var existing = DecisionService.GetShownTriggers(observable.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.RemoveShownTrigger(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveDecisionShownTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? addDecisionFailureTriggerCommand;
    public AsyncRelayCommand<object> AddDecisionFailureTriggerCommand => addDecisionFailureTriggerCommand ??= new(AddDecisionFailureTrigger, CanAddDecisionFailureTrigger);

    private async Task AddDecisionFailureTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var all = TriggerService.Get();
        var existing = DecisionService.GetFailureTriggers(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddFailureTrigger(owner, relation);

        //Setup();

        Selected = TriggerModelMap[relation];
    }
    private bool CanAddDecisionFailureTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? removeDecisionFailureTriggerCommand;
    public AsyncRelayCommand<object> RemoveDecisionFailureTriggerCommand => removeDecisionFailureTriggerCommand ??= new(RemoveDecisionFailureTrigger, CanRemoveDecisionFailureTrigger);

    private async Task RemoveDecisionFailureTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var existing = DecisionService.GetFailureTriggers(observable.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.RemoveFailureTrigger(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveDecisionFailureTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? addDecisionValidTriggerCommand;
    public AsyncRelayCommand<object> AddDecisionValidTriggerCommand => addDecisionValidTriggerCommand ??= new(AddDecisionValidTrigger, CanAddDecisionValidTrigger);

    private async Task AddDecisionValidTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var all = TriggerService.Get();
        var existing = DecisionService.GetValidTriggers(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.AddValidTrigger(owner, relation);

        //Setup();

        Selected = TriggerModelMap[relation];
    }
    private bool CanAddDecisionValidTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? removeDecisionValidTriggerCommand;
    public AsyncRelayCommand<object> RemoveDecisionValidTriggerCommand => removeDecisionValidTriggerCommand ??= new(RemoveDecisionValidTrigger, CanRemoveDecisionValidTrigger);

    private async Task RemoveDecisionValidTrigger(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var existing = DecisionService.GetValidTriggers(observable.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        DecisionService.RemoveValidTrigger(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveDecisionValidTrigger(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? addDecisionEffectCommand;
    public AsyncRelayCommand<object> AddDecisionEffectCommand => addDecisionEffectCommand ??= new(AddDecisionEffect, CanAddDecisionEffect);

    private async Task AddDecisionEffect(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var all = EffectService.Get();
        var existing = DecisionService.GetEffects(observable.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        DecisionService.AddEffect(owner, relation);

        //Setup();

        Selected = EffectModelMap[relation];
    }
    private bool CanAddDecisionEffect(object? param)
    {
        return param is DecisionViewModel;
    }

    private AsyncRelayCommand<object>? removeDecisionEffectCommand;
    public AsyncRelayCommand<object> RemoveDecisionEffectCommand => removeDecisionEffectCommand ??= new(RemoveDecisionEffect, CanRemoveDecisionEffect);

    private async Task RemoveDecisionEffect(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        var existing = DecisionService.GetEffects(observable.Model);

        Finder.Items = existing.Select(candidate => EffectModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Decision owner = observable.Model;
        Effect relation = ((EffectViewModel)Finder.Selected).Model;

        DecisionService.RemoveEffect(owner, relation);

        //Setup();

        Selected = null;
    }
    private bool CanRemoveDecisionEffect(object? param)
    {
        return param is DecisionViewModel;
    }

    #endregion


    #region Trigger Commands

    private RelayCommand? createTriggerCommand;
    public RelayCommand CreateTriggerCommand => createTriggerCommand ??= new(CreateTrigger);

    private void CreateTrigger()
    {
        var model = new Trigger();
        TriggerService.Insert(model);

        //Setup();

        var observable = TriggerModelMap[model];
        Selected = observable;
    }

    private RelayCommand<object>? deleteTriggerCommand;
    public RelayCommand<object> DeleteTriggerCommand => deleteTriggerCommand ??= new(DeleteTrigger, CanDeleteTrigger);

    private void DeleteTrigger(object? param)
    {
        if (param is not TriggerViewModel observable)
        { return; }

        var model = observable.Model;
        TriggerService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteTrigger(object? param)
    {
        return param is TriggerViewModel;
    }

    #endregion


    #region Effect Commands

    private RelayCommand? createEffectCommand;
    public RelayCommand CreateEffectCommand => createEffectCommand ??= new(CreateEffect);

    private void CreateEffect()
    {
        var model = new Effect();
        EffectService.Insert(model);

        //Setup();

        var observable = EffectModelMap[model];
        Selected = observable;
    }

    private RelayCommand<object>? deleteEffectCommand;
    public RelayCommand<object> DeleteEffectCommand => deleteEffectCommand ??= new(DeleteEffect, CanDeleteEffect);

    private void DeleteEffect(object? param)
    {
        if (param is not EffectViewModel observable)
        { return; }

        var model = observable.Model;
        EffectService.Delete(model);

        //Setup();

        Selected = null;
    }
    private bool CanDeleteEffect(object? param)
    {
        return param is EffectViewModel;
    }

    #endregion
}

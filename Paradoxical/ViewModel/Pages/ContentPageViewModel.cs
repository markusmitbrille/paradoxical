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

        var observable = ScriptModelMap[model];
        Selected = observable;

        var node = new ScriptBranch() { Observable = observable };
        ModNode.ScriptNodes.Add(node);

        InitScriptNode(node);
        InitScriptBranch(node);

        ModNode.Expand();
        ModNode.ScriptNodes.Expand();

        node.Select();
        node.Expand();
    }

    private RelayCommand<object>? deleteScriptCommand;
    public RelayCommand<object> DeleteScriptCommand => deleteScriptCommand ??= new(DeleteScript, CanDeleteScript);

    private void DeleteScript(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        if (Selected is ScriptViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        ScriptModelMap.Remove(model);
        ScriptService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<ScriptNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
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

        CreateEventPortrait(model, PortraitPosition.Left);
        CreateEventPortrait(model, PortraitPosition.Right);
        CreateEventPortrait(model, PortraitPosition.LowerLeft);
        CreateEventPortrait(model, PortraitPosition.LowerCenter);
        CreateEventPortrait(model, PortraitPosition.LowerRight);

        var observable = EventModelMap[model];
        Selected = observable;

        var node = new EventBranch() { Observable = observable };
        ModNode.EventNodes.Add(node);

        InitEventNode(node);
        InitEventBranch(node);

        ModNode.Expand();
        ModNode.EventNodes.Expand();

        node.Select();
    }

    private void CreateEventPortrait(Event parent, PortraitPosition position)
    {
        var model = new Portrait() { EventId = parent.Id, Position = position };
        PortraitService.Insert(model);
    }

    private RelayCommand<object>? deleteEventCommand;
    public RelayCommand<object> DeleteEventCommand => deleteEventCommand ??= new(DeleteEvent, CanDeleteEvent);

    private void DeleteEvent(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        if (Selected is EventViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        EventModelMap.Remove(model);
        EventService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<EventNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
    }
    private bool CanDeleteEvent(object? param)
    {
        return param is EventViewModel;
    }

    private RelayCommand<object>? createEventOptionCommand;
    public RelayCommand<object> CreateEventOptionCommand => createEventOptionCommand ??= new(CreateEventOption, CanCreateEventOption);

    private void CreateEventOption(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        Option relation = new() { EventId = observable.Id };
        OptionService.Insert(relation);

        var relationViewModel = OptionModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new OptionBranch() { Observable = relationViewModel };

            InitOptionNode(child);
            InitOptionBranch(child);

            parent.OptionNodes.Add(child);
        }
    }
    private bool CanCreateEventOption(object? param)
    {
        return param is EventViewModel;
    }

    private RelayCommand<object>? createEventOnionCommand;
    public RelayCommand<object> CreateEventOnionCommand => createEventOnionCommand ??= new(CreateEventOnion, CanCreateEventOnion);

    private void CreateEventOnion(object? param)
    {
        if (param is not EventViewModel observable)
        { return; }

        Onion relation = new() { EventId = observable.Id };
        OnionService.Insert(relation);

        var relationViewModel = OnionModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new OnionBranch() { Observable = relationViewModel };

            InitOnionNode(child);
            InitOnionBranch(child);

            parent.OnionNodes.Add(child);
        }
    }
    private bool CanCreateEventOnion(object? param)
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

        var relationViewModel = TriggerModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new TriggerLeaf() { Observable = relationViewModel };

            InitTriggerNode(child);

            parent.TriggerNodes.Add(child);
        }
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

        var relationViewModel = TriggerModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.TriggerNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = EffectModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new EffectLeaf() { Observable = relationViewModel };

            InitEffectNode(child);

            parent.ImmediateEffectNodes.Add(child);
        }
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

        var relationViewModel = EffectModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<EffectNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.ImmediateEffectNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = EffectModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new EffectLeaf() { Observable = relationViewModel };

            InitEffectNode(child);

            parent.AfterEffectNodes.Add(child);
        }
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

        var relationViewModel = EffectModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<EffectNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.AfterEffectNodes.RemoveAll(child => nodes.Contains(child));
        }
    }
    private bool CanRemoveEventAfterEffect(object? param)
    {
        return param is EventViewModel;
    }

    #endregion


    #region Portrait Commands
    #endregion


    #region Option Commands

    private RelayCommand<object>? deleteOptionCommand;
    public RelayCommand<object> DeleteOptionCommand => deleteOptionCommand ??= new(DeleteOption, CanDeleteOption);

    private void DeleteOption(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        if (Selected is OptionViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        OptionModelMap.Remove(model);
        OptionService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<OptionNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
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

        var relationViewModel = TriggerModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new TriggerLeaf() { Observable = relationViewModel };

            InitTriggerNode(child);

            parent.TriggerNodes.Add(child);
        }
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

        var relationViewModel = TriggerModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.TriggerNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = EffectModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new EffectLeaf() { Observable = relationViewModel };

            InitEffectNode(child);

            parent.EffectNodes.Add(child);
        }
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

        var relationViewModel = EffectModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<EffectNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.EffectNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        if (Selected is OnionViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        OnionModelMap.Remove(model);
        OnionService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<OnionNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
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

        var observable = DecisionModelMap[model];
        Selected = observable;

        var node = new DecisionBranch() { Observable = observable };
        ModNode.DecisionNodes.Add(node);

        InitDecisionNode(node);
        InitDecisionBranch(node);

        ModNode.Expand();
        ModNode.DecisionNodes.Expand();

        node.Select();
    }

    private RelayCommand<object>? deleteDecisionCommand;
    public RelayCommand<object> DeleteDecisionCommand => deleteDecisionCommand ??= new(DeleteDecision, CanDeleteDecision);

    private void DeleteDecision(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        if (Selected is DecisionViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        DecisionModelMap.Remove(model);
        DecisionService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<DecisionNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
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

        var relationViewModel = TriggerModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new TriggerLeaf() { Observable = relationViewModel };

            InitTriggerNode(child);

            parent.ShownTriggerNodes.Add(child);
        }
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

        var relationViewModel = TriggerModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.ShownTriggerNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = TriggerModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new TriggerLeaf() { Observable = relationViewModel };

            InitTriggerNode(child);

            parent.FailureTriggerNodes.Add(child);
        }
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

        var relationViewModel = TriggerModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.FailureTriggerNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = TriggerModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new TriggerLeaf() { Observable = relationViewModel };

            InitTriggerNode(child);

            parent.ValidTriggerNodes.Add(child);
        }
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

        var relationViewModel = TriggerModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.ValidTriggerNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var relationViewModel = EffectModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var child = new EffectLeaf() { Observable = relationViewModel };

            InitEffectNode(child);

            parent.EffectNodes.Add(child);
        }
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

        var relationViewModel = EffectModelMap[relation];

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        var nodes = RootNode.Descendants
            .OfType<EffectNode>()
            .Where(node => node.Observable == relationViewModel)
            .ToArray();

        foreach (var parent in parents)
        {
            parent.EffectNodes.RemoveAll(child => nodes.Contains(child));
        }
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

        var observable = TriggerModelMap[model];
        Selected = observable;

        var node = new TriggerBranch() { Observable = observable };
        ModNode.TriggerNodes.Add(node);

        InitTriggerNode(node);
        InitTriggerBranch(node);

        ModNode.Expand();
        ModNode.TriggerNodes.Expand();

        node.Select();
    }

    private RelayCommand<object>? deleteTriggerCommand;
    public RelayCommand<object> DeleteTriggerCommand => deleteTriggerCommand ??= new(DeleteTrigger, CanDeleteTrigger);

    private void DeleteTrigger(object? param)
    {
        if (param is not TriggerViewModel observable)
        { return; }

        if (Selected is TriggerViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        TriggerModelMap.Remove(model);
        TriggerService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<TriggerNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
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

        var observable = EffectModelMap[model];
        Selected = observable;

        var node = new EffectBranch() { Observable = observable };
        ModNode.EffectNodes.Add(node);

        InitEffectNode(node);
        InitEffectBranch(node);

        ModNode.Expand();
        ModNode.EffectNodes.Expand();

        node.Select();
    }

    private RelayCommand<object>? deleteEffectCommand;
    public RelayCommand<object> DeleteEffectCommand => deleteEffectCommand ??= new(DeleteEffect, CanDeleteEffect);

    private void DeleteEffect(object? param)
    {
        if (param is not EffectViewModel observable)
        { return; }

        if (Selected is EffectViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        EffectModelMap.Remove(model);
        EffectService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<EffectNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
    }
    private bool CanDeleteEffect(object? param)
    {
        return param is EffectViewModel;
    }

    #endregion
}

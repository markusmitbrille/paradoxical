using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
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

    public ModNode ModNode { get; set; } = new();

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
        Setup();

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


    #region Setup

    private void Setup()
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

        SetupModNode();

        SetupScriptNodes();
        SetupEventNodes();
        SetupDecisionNodes();
        SetupTriggerNodes();
        SetupEffectNodes();
    }

    private void SetupModNode()
    {
        ModNode = new() { Observable = ModViewModel };

        ModNode.EditCommand = EditCommand;

        ModNode.CreateScriptCommand = CreateScriptCommand;
        ModNode.CreateEventCommand = CreateEventCommand;
        ModNode.CreateDecisionCommand = CreateDecisionCommand;
        ModNode.CreateTriggerCommand = CreateTriggerCommand;
        ModNode.CreateEffectCommand = CreateEffectCommand;

        RootNode = new CollectionNode();
        RootNode.Add(ModNode);
    }

    private void SetupScriptNodes()
    {
        ModNode.ScriptNodes.Header = "Scripts";

        foreach (var observable in ScriptModelMap.Wrappers)
        {
            ScriptNode node = new() { Observable = observable };

            node.DeleteCommand = DeleteScriptCommand;
            node.EditCommand = EditCommand;

            ModNode.ScriptNodes.Add(node);
        }
    }

    private void SetupEventNodes()
    {
        ModNode.EventNodes.Header = "Events";

        foreach (var observable in EventModelMap.Wrappers)
        {
            EventNode node = new() { Observable = observable };

            node.DeleteCommand = DeleteEventCommand;
            node.EditCommand = EditCommand;

            node.CreatePortraitCommand = CreateEventPortraitCommand;
            node.CreateOptionCommand = CreateEventOptionCommand;
            node.CreateOnionCommand = CreateEventOnionCommand;

            node.AddTriggerCommand = AddEventTriggerCommand;
            node.RemoveTriggerCommand = RemoveEventTriggerCommand;
            // setup other commands

            SetupEventPortraitNodes(node);
            SetupEventOptionNodes(node);
            SetupEventOnionNodes(node);

            SetupEventTriggerNodes(node);
            // setup other nodes

            ModNode.EventNodes.Add(node);
        }
    }

    private void SetupEventPortraitNodes(EventNode parent)
    {
        parent.PortraitNodes.Header = "Portraits";

        var relations = EventService.GetPortraits(parent.Observable.Model);
        foreach (var relation in relations)
        {
            var observable = PortraitModelMap[relation];

            PortraitNode child = new() { Observable = observable };
            parent.PortraitNodes.Add(child);
        }
    }

    private void SetupEventOptionNodes(EventNode parent)
    {
        parent.OptionNodes.Header = "Options";

        var relations = EventService.GetOptions(parent.Observable.Model);
        foreach (var relation in relations)
        {
            var observable = OptionModelMap[relation];

            OptionNode child = new() { Observable = observable };
            parent.OptionNodes.Add(child);
        }
    }

    private void SetupEventOnionNodes(EventNode parent)
    {
        parent.OnionNodes.Header = "On-Actions";

        var relations = EventService.GetOnions(parent.Observable.Model);
        foreach (var relation in relations)
        {
            var observable = OnionModelMap[relation];

            OnionNode child = new() { Observable = observable };
            parent.OnionNodes.Add(child);
        }
    }

    private void SetupEventTriggerNodes(EventNode parent)
    {
        parent.TriggerNodes.Header = "Triggers";

        var relations = EventService.GetTriggers(parent.Observable.Model);
        foreach (var relation in relations)
        {
            var observable = TriggerModelMap[relation];
            TriggerNode child = new() { Observable = observable };

            child.DeleteCommand = DeleteTriggerCommand;
            child.EditCommand = EditCommand;

            parent.TriggerNodes.Add(child);
        }
    }

    private void SetupDecisionNodes()
    {
        ModNode.DecisionNodes.Header = "Decisions";

        foreach (var observable in DecisionModelMap.Wrappers)
        {
            DecisionNode node = new() { Observable = observable };

            node.DeleteCommand = DeleteDecisionCommand;
            node.EditCommand = EditCommand;
            // setup other commands

            // setup other nodes

            ModNode.DecisionNodes.Add(node);
        }
    }

    private void SetupTriggerNodes()
    {
        ModNode.TriggerNodes.Header = "Triggers";

        foreach (var observable in TriggerModelMap.Wrappers)
        {
            TriggerNode node = new() { Observable = observable };

            node.DeleteCommand = DeleteTriggerCommand;
            node.EditCommand = EditCommand;

            ModNode.TriggerNodes.Add(node);
        }
    }

    private void SetupEffectNodes()
    {
        ModNode.EffectNodes.Header = "Effects";

        foreach (var observable in EffectModelMap.Wrappers)
        {
            EffectNode node = new() { Observable = observable };

            node.DeleteCommand = DeleteEffectCommand;
            node.EditCommand = EditCommand;

            ModNode.EffectNodes.Add(node);
        }
    }

    #endregion


    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        var model = new Script();
        ScriptService.Insert(model);

        Setup();

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

        Setup();

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

        Setup();

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

        Setup();

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

        Setup();

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

        Setup();

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

        Setup();

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
        if (param is not EventViewModel evt)
        { return; }

        var all = TriggerService.Get();
        var existing = EventService.GetTriggers(evt.Model);
        var candidates = all.Except(existing);

        Finder.Items = candidates.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = evt.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        EventService.AddTrigger(owner, relation);

        Setup();

        var observable = TriggerModelMap[relation];
        Selected = observable;
    }
    private bool CanAddEventTrigger(object? param)
    {
        return param is EventViewModel;
    }

    private AsyncRelayCommand<object>? removeEventTriggerCommand;
    public AsyncRelayCommand<object> RemoveEventTriggerCommand => removeEventTriggerCommand ??= new(RemoveEventTrigger, CanRemoveEventTrigger);

    private async Task RemoveEventTrigger(object? param)
    {
        if (param is not EventViewModel evt)
        { return; }

        var existing = EventService.GetTriggers(evt.Model);

        Finder.Items = existing.Select(candidate => TriggerModelMap[candidate]);

        await Finder.Show();

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Event owner = evt.Model;
        Trigger relation = ((TriggerViewModel)Finder.Selected).Model;

        EventService.RemoveTrigger(owner, relation);

        Setup();

        Selected = null;
    }
    private bool CanRemoveEventTrigger(object? param)
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

        Setup();

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

        Setup();

        Selected = null;
    }
    private bool CanDeleteOption(object? param)
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

        Setup();

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

        Setup();

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

        Setup();

        Selected = null;
    }
    private bool CanDeleteDecision(object? param)
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

        Setup();

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

        Setup();

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

        Setup();

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

        Setup();

        Selected = null;
    }
    private bool CanDeleteEffect(object? param)
    {
        return param is EffectViewModel;
    }

    #endregion
}

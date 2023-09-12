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
using System.Linq;
using System.Windows.Controls;

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

    private NodeMap<ScriptViewModel, ScriptNode> ScriptNodeMap { get; set; } = new();
    private NodeMap<EventViewModel, EventNode> EventNodeMap { get; set; } = new();
    private NodeMap<OptionViewModel, OptionNode> OptionNodeMap { get; set; } = new();
    private NodeMap<OnionViewModel, OnionNode> OnionNodeMap { get; set; } = new();
    private NodeMap<PortraitViewModel, PortraitNode> PortraitNodeMap { get; set; } = new();
    private NodeMap<DecisionViewModel, DecisionNode> DecisionNodeMap { get; set; } = new();
    private NodeMap<TriggerViewModel, TriggerNode> TriggerNodeMap { get; set; } = new();
    private NodeMap<EffectViewModel, EffectNode> EffectNodeMap { get; set; } = new();

    private Dictionary<Script, ScriptNode> ScriptNodes { get; set; } = new();

    private ObservableObject? selected;
    public ObservableObject? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    private Node? selectedNode;
    public Node? SelectedNode
    {
        get => selectedNode;
        set
        {
            SetProperty(ref selectedNode, value);

            if (value is IObservableWrapper wrapper)
            {
                Selected = wrapper.Observable;
            }
        }
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
        IEffectService effectService)
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

        ScriptNodeMap = new(ScriptModelMap.Wrappers);
        EventNodeMap = new(EventModelMap.Wrappers);
        OptionNodeMap = new(OptionModelMap.Wrappers);
        OnionNodeMap = new(OnionModelMap.Wrappers);
        PortraitNodeMap = new(PortraitModelMap.Wrappers);
        DecisionNodeMap = new(DecisionModelMap.Wrappers);
        TriggerNodeMap = new(TriggerModelMap.Wrappers);
        EffectNodeMap = new(EffectModelMap.Wrappers);

        RootNode = new CollectionNode();
        RootNode.Add(ModNode);

        SetupModNode(ModNode);
        SetupScriptNodes();

        // setup other nodes

        DataService.BeginTransaction();
    }

    private void SetupModNode(ModNode node)
    {
        node.EditCommand = EditCommand;
        node.CreateScriptCommand = CreateScriptCommand;
        // setup other commands

        node.ScriptNodes.Header = "Scripts";
        foreach (var observable in ScriptModelMap.Wrappers)
        {
            var child = ScriptNodeMap[observable];
            node.ScriptNodes.Add(child);
        }
    }

    private void SetupScriptNodes()
    {
        foreach (var node in ScriptNodeMap.Nodes)
        {
            SetupScriptNode(node);
        }
    }

    private void SetupScriptNode(ScriptNode node)
    {
        node.DeleteCommand = DeleteScriptCommand;
        node.EditCommand = EditCommand;
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

    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        for (int i = 0; i < 200; i++)
        {
            var model = new Script();
            ScriptService.Insert(model);

            var observable = ScriptModelMap[model];
            var node = ScriptNodeMap[observable];

            SetupScriptNode(node);

            ModNode.ScriptNodes.Add(node);
        }
    }

    private RelayCommand<object>? deleteScriptCommand;
    public RelayCommand<object> DeleteScriptCommand => deleteScriptCommand ??= new(DeleteScript, CanDeleteScript);

    private void DeleteScript(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        var node = ScriptNodeMap[observable];

        ModNode.ScriptNodes.Remove(node);

        var model = observable.Model;
        ScriptService.Delete(model);

        ScriptModelMap.Remove(model);
        ScriptNodeMap.Remove(observable);
    }
    private bool CanDeleteScript(object? param)
    {
        return param is ScriptViewModel;
    }

    #endregion


    #region Event Commands



    #endregion
}

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

public class ModelMap<M, W>
    where M : IModel, new()
    where W : IModelWrapper<M>, new()
{
    private Dictionary<M, W> Map { get; } = new();

    public W this[M model]
    {
        get
        {
            if (Map.ContainsKey(model) == true)
            {
                return Map[model];
            }

            var wrapper = new W() { Model = model };
            Map[model] = wrapper;

            return wrapper;
        }
    }

    public IEnumerable<M> Models => Map.Keys;
    public IEnumerable<W> Wrappers => Map.Values;

    public ModelMap()
    {
    }

    public ModelMap(IEnumerable<M> models)
    {
        Map = models.ToDictionary(model => model, model => new W() { Model = model });
    }

    public void Remove(M model)
    {
        Map.Remove(model);
    }
}

public class NodeMap<O, N>
    where O : ObservableObject, new()
    where N : ObservableNode<O>, new()
{
    private Dictionary<O, N> Map { get; } = new();

    public N this[O observable]
    {
        get
        {
            if (Map.ContainsKey(observable) == true)
            {
                return Map[observable];
            }

            var node = new N() { Observable = observable };
            Map[observable] = node;

            return node;
        }
    }

    public IEnumerable<O> Observables => Map.Keys;
    public IEnumerable<N> Nodes => Map.Values;

    public NodeMap()
    {
    }

    public NodeMap(IEnumerable<O> observables)
    {
        Map = observables.ToDictionary(observable => observable, observable => new N() { Observable = observable });
    }

    public void Remove(O observable)
    {
        Map.Remove(observable);
    }
}

public abstract class Node : ObservableObject
{
    private string header = string.Empty;
    public string Header
    {
        get => header;
        set => SetProperty(ref header, value);
    }

    public abstract IEnumerable<Node> Children { get; }
}

public abstract class ObservableNode<T> : Node
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();

    public override IEnumerable<Node> Children => Enumerable.Empty<Node>();
}

public class CollectionNode : Node
{
    private readonly ObservableCollection<Node> children = new();
    public sealed override IEnumerable<Node> Children => children;

    public void Add(Node node) => children.Add(node);
    public void Remove(Node node) => children.Remove(node);
    public void Clear() => children.Clear();
}


public class ModNode : ObservableNode<ModViewModel>
{
    public CollectionNode ScriptNodes { get; } = new();
    public CollectionNode EventNodes { get; } = new();
    public CollectionNode DecisionNodes { get; } = new();
    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public RelayCommand? CreateScriptCommand { get; set; }
    public RelayCommand? CreateEventCommand { get; set; }
    public RelayCommand? CreateDecisionCommand { get; set; }
    public RelayCommand? CreateTriggerCommand { get; set; }
    public RelayCommand? CreateEffectCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return ScriptNodes;
            yield return EventNodes;
            yield return DecisionNodes;
            yield return TriggerNodes;
            yield return EffectNodes;
        }
    }
}


public class ScriptNode : ObservableNode<ScriptViewModel>
{
    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}


public class EventNode : ObservableNode<EventViewModel>
{
    public CollectionNode PortraitNodes { get; } = new();
    public CollectionNode OptionNodes { get; } = new();
    public CollectionNode OnionNodes { get; } = new();

    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode ImmediateEffectNodes { get; } = new();
    public CollectionNode AfterEffectNodes { get; } = new();

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }

    public RelayCommand<object>? CreatePortraitCommand { get; set; }
    public RelayCommand<object>? CreateOptionCommand { get; set; }
    public RelayCommand<object>? CreateOnionCommand { get; set; }

    public RelayCommand<object>? AddTriggerCommand { get; set; }
    public RelayCommand<object>? RemoveTriggerCommand { get; set; }

    public RelayCommand<object>? AddImmediateEffectCommand { get; set; }
    public RelayCommand<object>? RemoveImmediateEffectCommand { get; set; }

    public RelayCommand<object>? AddAfterEffectCommand { get; set; }
    public RelayCommand<object>? RemoveAfterEffectCommand { get; set; }

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return PortraitNodes;
            yield return OptionNodes;
            yield return OnionNodes;

            yield return TriggerNodes;
            yield return ImmediateEffectNodes;
            yield return AfterEffectNodes;
        }
    }
}


public class PortraitNode : ObservableNode<PortraitViewModel>
{
}


public class OptionNode : ObservableNode<OptionViewModel>
{
    public CollectionNode TriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return TriggerNodes;
            yield return EffectNodes;
        }
    }
}


public class DecisionNode : ObservableNode<DecisionViewModel>
{
    public CollectionNode ShownTriggerNodes { get; } = new();
    public CollectionNode FailureTriggerNodes { get; } = new();
    public CollectionNode ValidTriggerNodes { get; } = new();
    public CollectionNode EffectNodes { get; } = new();

    public override IEnumerable<Node> Children
    {
        get
        {
            yield return ShownTriggerNodes;
            yield return FailureTriggerNodes;
            yield return ValidTriggerNodes;
            yield return EffectNodes;
        }
    }
}


public class OnionNode : ObservableNode<OnionViewModel>
{
}


public class TriggerNode : ObservableNode<TriggerViewModel>
{
}


public class EffectNode : ObservableNode<EffectViewModel>
{
}


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

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

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

        RootNode.Add(ModNode);

        SetupModNode();

        // setup other node collections

        DataService.BeginTransaction();
    }

    private void SetupModNode()
    {
        ModNode.CreateScriptCommand = CreateScriptCommand;
        // setup other commands

        ModNode.ScriptNodes.Header = "Scripts";
        foreach (var observable in ScriptModelMap.Wrappers)
        {
            var node = ScriptNodeMap[observable];
            ModNode.ScriptNodes.Add(node);
        }
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

    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        var model = new Script();
        ScriptService.Insert(model);

        var observable = ScriptModelMap[model];
        var node = ScriptNodeMap[observable];

        ModNode.ScriptNodes.Add(node);
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
}

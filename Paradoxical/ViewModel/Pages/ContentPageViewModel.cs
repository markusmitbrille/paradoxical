using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

public interface INode : IEnumerable<INode>
{
    public string Header { get; init; }
}

public abstract class CollectionNode<T> : ObservableCollection<ObservableNode<T>>, INode
    where T : ObservableObject, new()
{
    public abstract string Header { get; init; }

    IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class ObservableNode<T> : INode
    where T : ObservableObject, new()
{
    public abstract string Header { get; init; }
    public T Observable { get; init; } = new();

    public virtual IEnumerator<INode> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public class ModNode : ObservableNode<ModViewModel>
{
    public override string Header { get; init; } = "Mod";

    public ScriptCollectionNode ScriptNodes { get; } = new();
    public EventCollectionNode EventNodes { get; } = new();
    public DecisionCollectionNode DecisionNodes { get; } = new();
    public TriggerCollectionNode TriggerNodes { get; } = new();
    public EffectCollectionNode EffectNodes { get; } = new();

    public override IEnumerator<INode> GetEnumerator()
    {
        yield return ScriptNodes;
        yield return EventNodes;
        yield return DecisionNodes;
        yield return TriggerNodes;
        yield return EffectNodes;
    }
}


public class ScriptCollectionNode : CollectionNode<ScriptViewModel>
{
    public override string Header { get; init; } = "Scripts";
}

public class ScriptNode : ObservableNode<ScriptViewModel>
{
    public override string Header { get; init; } = "Script";
}


public class EventCollectionNode : CollectionNode<EventViewModel>
{
    public override string Header { get; init; } = "Events";
}

public class EventNode : ObservableNode<EventViewModel>
{
    public override string Header { get; init; } = "Event";

    public PortraitCollectionNode PortraitNodes { get; } = new();
    public OptionCollectionNode OptionNodes { get; } = new();
    public OnionCollectionNode OnionNodes { get; } = new();

    public TriggerCollectionNode TriggerNodes { get; } = new();
    public EffectCollectionNode ImmediateEffectNodes { get; } = new() { Header = "Immediate Effects" };
    public EffectCollectionNode AfterEffectNodes { get; } = new() { Header = "After Effects" };

    public override IEnumerator<INode> GetEnumerator()
    {
        yield return PortraitNodes;
        yield return OptionNodes;
        yield return OnionNodes;

        yield return TriggerNodes;
        yield return ImmediateEffectNodes;
        yield return AfterEffectNodes;
    }
}


public class PortraitCollectionNode : CollectionNode<PortraitViewModel>
{
    public override string Header { get; init; } = "Portraits";
}

public class PortraitNode : ObservableNode<PortraitViewModel>
{
    public override string Header { get; init; } = "Portrait";

    public RelayCommand<object>? EditCommand { get; set; }
    public RelayCommand<object>? DeleteCommand { get; set; }
}


public class OptionCollectionNode : CollectionNode<OptionViewModel>
{
    public override string Header { get; init; } = "Options";
}

public class OptionNode : ObservableNode<OptionViewModel>
{
    public override string Header { get; init; } = "Option";

    public TriggerCollectionNode TriggerNodes { get; } = new();
    public EffectCollectionNode EffectNodes { get; } = new();

    public override IEnumerator<INode> GetEnumerator()
    {
        yield return TriggerNodes;
        yield return EffectNodes;
    }
}


public class DecisionCollectionNode : CollectionNode<DecisionViewModel>
{
    public override string Header { get; init; } = "Decisions";
}

public class DecisionNode : ObservableNode<DecisionViewModel>
{
    public override string Header { get; init; } = "Decision";

    public TriggerCollectionNode ShownTriggerNodes { get; } = new();
    public TriggerCollectionNode FailureTriggerNodes { get; } = new();
    public TriggerCollectionNode ValidTriggerNodes { get; } = new();
    public EffectCollectionNode EffectNodes { get; } = new();

    public override IEnumerator<INode> GetEnumerator()
    {
        yield return ShownTriggerNodes;
        yield return FailureTriggerNodes;
        yield return ValidTriggerNodes;
        yield return EffectNodes;
    }
}


public class OnionCollectionNode : CollectionNode<OnionViewModel>
{
    public override string Header { get; init; } = "On-Actions";
}

public class OnionNode : ObservableNode<OnionViewModel>
{
    public override string Header { get; init; } = "On-Action";
}


public class TriggerCollectionNode : CollectionNode<TriggerViewModel>
{
    public override string Header { get; init; } = "Triggers";
}

public class TriggerNode : ObservableNode<TriggerViewModel>
{
    public override string Header { get; init; } = "Trigger";
}


public class EffectCollectionNode : CollectionNode<EffectViewModel>
{
    public override string Header { get; init; } = "Effects";
}

public class EffectNode : ObservableNode<EffectViewModel>
{
    public override string Header { get; init; } = "Effect";
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

    private ModNode rootNode = new();
    public ModNode RootNode
    {
        get => rootNode;
        set => SetProperty(ref rootNode, value);
    }

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

        RootNode = new() { Observable = ModViewModel };

        ScriptNodeMap = new(ScriptModelMap.Wrappers);
        EventNodeMap = new(EventModelMap.Wrappers);
        OptionNodeMap = new(OptionModelMap.Wrappers);
        OnionNodeMap = new(OnionModelMap.Wrappers);
        PortraitNodeMap = new(PortraitModelMap.Wrappers);
        DecisionNodeMap = new(DecisionModelMap.Wrappers);
        TriggerNodeMap = new(TriggerModelMap.Wrappers);
        EffectNodeMap = new(EffectModelMap.Wrappers);

        // configure nodes, add subnodes

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

    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        Script model = new();
        ScriptService.Insert(model);

        ScriptViewModel observable = ScriptModelMap[model];
        ScriptNode node = ScriptNodeMap[observable];

        RootNode.ScriptNodes.Add(node);
    }

    private RelayCommand<object>? deleteScriptCommand;
    public RelayCommand<object> DeleteScriptCommand => deleteScriptCommand ??= new(DeleteScript, CanDeleteScript);

    private void DeleteScript(object? param)
    {
        if (param is not ScriptViewModel observable)
        { return; }

        Script model = observable.Model;
        ScriptService.Delete(model);

        ScriptModelMap.Remove(model);
        ScriptNodeMap.Remove(observable);

        RootNode.ScriptNodes.RemoveAll(node => node.Observable == observable);
    }
    private bool CanDeleteScript(object? param)
    {
        return param is ScriptViewModel;
    }

    #endregion
}

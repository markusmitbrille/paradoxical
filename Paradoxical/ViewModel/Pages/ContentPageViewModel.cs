using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Paradoxical.ViewModel;

public class ModelMap<M, W> : Dictionary<M, W>
    where M : IModel, new()
    where W : IModelWrapper<M>, new()
{
    public IEnumerable<M> Models => Keys;
    public IEnumerable<W> Wrappers => Values;

    public ModelMap()
    {
    }

    public ModelMap(IEnumerable<M> models)
        : base(models.ToDictionary(model => model, model => new W() { Model = model }))
    {
    }

    public W Wrap(M model)
    {
        if (ContainsKey(model) == true)
        {
            return this[model];
        }

        var wrapper = new W() { Model = model };
        this[model] = wrapper;

        return wrapper;
    }
}

public abstract class ObservableNode<T>
    where T : ObservableObject, new()
{
    public T Observable { get; init; } = new();
}

public abstract class ObservableContainerNode<T> : ObservableCollection<ObservableNode<T>>
    where T : ObservableObject, new()
{
    public abstract string Header { get; init; }
}

public abstract class ObservableLeafNode<T> : ObservableNode<T>
    where T : ObservableObject, new()
{
}

public abstract class ObservableRootNode<T> : ObservableNode<T>, IEnumerable
    where T : ObservableObject, new()
{
    public abstract IEnumerator GetEnumerator();
}


public class ModRootNode : ObservableRootNode<ModViewModel>
{
    public ScriptContainerNode ScriptNodes { get; } = new();
    public EventContainerNode EventNodes { get; } = new();
    public DecisionContainerNode DecisionNodes { get; } = new();
    public TriggerContainerNode TriggerNodes { get; } = new();
    public EffectContainerNode EffectNodes { get; } = new();

    public override IEnumerator GetEnumerator()
    {
        yield return ScriptNodes;
        yield return EventNodes;
        yield return DecisionNodes;
        yield return TriggerNodes;
        yield return EffectNodes;
    }
}


public class ScriptContainerNode : ObservableContainerNode<ScriptViewModel>
{
    public override string Header { get; init; } = "Scripts";

    public RelayCommand? CreateScriptCommand { get; set; }
}

public class ScriptLeafNode : ObservableLeafNode<ScriptViewModel>
{
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public class ScriptRootNode : ObservableRootNode<ScriptViewModel>
{
    public RelayCommand<object>? DeleteScriptCommand { get; set; }

    public override IEnumerator GetEnumerator()
    {
        yield break;
    }
}


public class EventContainerNode : ObservableContainerNode<EventViewModel>
{
    public override string Header { get; init; } = "Events";

    public RelayCommand? CreateEventCommand { get; set; }
}

public class EventLeafNode : ObservableLeafNode<EventViewModel>
{
    public RelayCommand<object>? DeleteCommand { get; set; }
}

public class EventRootNode : ObservableRootNode<EventViewModel>
{
    public PortraitContainerNode PortraitNodes { get; } = new();
    public OptionContainerNode OptionNodes { get; } = new();
    public OnionContainerNode OnionNodes { get; } = new();

    public TriggerContainerNode TriggerNodes { get; } = new();
    public EffectContainerNode ImmediateEffectNodes { get; } = new() { Header = "Immediate Effects" };
    public EffectContainerNode AfterEffectNodes { get; } = new() { Header = "After Effects" };

    public RelayCommand<object>? DeleteEventCommand { get; set; }

    public override IEnumerator GetEnumerator()
    {
        yield return PortraitNodes;
        yield return OptionNodes;
        yield return OnionNodes;

        yield return TriggerNodes;
        yield return ImmediateEffectNodes;
        yield return AfterEffectNodes;
    }
}


public class PortraitContainerNode : ObservableContainerNode<PortraitViewModel>
{
    public override string Header { get; init; } = "Portraits";
}

public class PortraitLeafNode : ObservableLeafNode<PortraitViewModel>
{
}

public class PortraitRootNode : ObservableRootNode<PortraitViewModel>
{
    public override IEnumerator GetEnumerator()
    {
        yield break;
    }
}


public class OptionContainerNode : ObservableContainerNode<OptionViewModel>
{
    public override string Header { get; init; } = "Options";
}

public class OptionLeafNode : ObservableLeafNode<OptionViewModel>
{
}

public class OptionRootNode : ObservableRootNode<OptionViewModel>
{
    public TriggerContainerNode TriggerNodes { get; } = new();
    public EffectContainerNode EffectNodes { get; } = new();

    public override IEnumerator GetEnumerator()
    {
        yield return TriggerNodes;
        yield return EffectNodes;
    }
}


public class DecisionContainerNode : ObservableContainerNode<DecisionViewModel>
{
    public override string Header { get; init; } = "Decisions";
}

public class DecisionLeafNode : ObservableLeafNode<DecisionViewModel>
{
}

public class DecisionRootNode : ObservableRootNode<DecisionViewModel>
{
    public TriggerContainerNode ShownTriggerNodes { get; } = new();
    public TriggerContainerNode FailureTriggerNodes { get; } = new();
    public TriggerContainerNode ValidTriggerNodes { get; } = new();
    public EffectContainerNode EffectNodes { get; } = new();

    public override IEnumerator GetEnumerator()
    {
        yield return ShownTriggerNodes;
        yield return FailureTriggerNodes;
        yield return ValidTriggerNodes;
        yield return EffectNodes;
    }
}


public class OnionContainerNode : ObservableContainerNode<OnionViewModel>
{
    public override string Header { get; init; } = "On-Actions";
}

public class OnionLeafNode : ObservableLeafNode<OnionViewModel>
{
}

public class OnionRootNode : ObservableRootNode<OnionViewModel>
{
    public override IEnumerator GetEnumerator()
    {
        yield break;
    }
}


public class TriggerContainerNode : ObservableContainerNode<TriggerViewModel>
{
    public override string Header { get; init; } = "Triggers";
}

public class TriggerLeafNode : ObservableLeafNode<TriggerViewModel>
{
}

public class TriggerRootNode : ObservableRootNode<TriggerViewModel>
{
    public override IEnumerator GetEnumerator()
    {
        yield break;
    }
}


public class EffectContainerNode : ObservableContainerNode<EffectViewModel>
{
    public override string Header { get; init; } = "Effects";
}

public class EffectLeafNode : ObservableLeafNode<EffectViewModel>
{
}

public class EffectRootNode : ObservableRootNode<EffectViewModel>
{
    public override IEnumerator GetEnumerator()
    {
        yield break;
    }
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

    private ModelMap<Script, ScriptViewModel> ScriptMap { get; set; } = new();
    private ModelMap<Event, EventViewModel> EventMap { get; set; } = new();
    private ModelMap<Option, OptionViewModel> OptionMap { get; set; } = new();
    private ModelMap<Onion, OnionViewModel> OnionMap { get; set; } = new();
    private ModelMap<Portrait, PortraitViewModel> PortraitMap { get; set; } = new();
    private ModelMap<Decision, DecisionViewModel> DecisionMap { get; set; } = new();
    private ModelMap<Trigger, TriggerViewModel> TriggerMap { get; set; } = new();
    private ModelMap<Effect, EffectViewModel> EffectMap { get; set; } = new();

    private ModRootNode rootNode = new();
    public ModRootNode RootNode
    {
        get => rootNode;
        set => SetProperty(ref rootNode, value);
    }

    private Dictionary<Script, ScriptRootNode> ScriptNodes { get; set; } = new();

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

        ScriptMap = new(ScriptService.Get());
        EventMap = new(EventService.Get());
        OptionMap = new(OptionService.Get());
        OnionMap = new(OnionService.Get());
        PortraitMap = new(PortraitService.Get());
        DecisionMap = new(DecisionService.Get());
        TriggerMap = new(TriggerService.Get());
        EffectMap = new(EffectService.Get());

        RootNode = new() { Observable = ModViewModel };

        BuildScriptNodes();

        DataService.BeginTransaction();
    }

    private void BuildScriptNodes()
    {
        foreach (var (_, observable) in ScriptMap)
        {
            var node = new ScriptRootNode() { Observable = observable, };
            RootNode.ScriptNodes.Add(node);
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

        ScriptService.UpdateAll(ScriptMap.Models);
        EventService.UpdateAll(EventMap.Models);
        OptionService.UpdateAll(OptionMap.Models);
        OnionService.UpdateAll(OnionMap.Models);
        PortraitService.UpdateAll(PortraitMap.Models);
        DecisionService.UpdateAll(DecisionMap.Models);
        TriggerService.UpdateAll(TriggerMap.Models);
        EffectService.UpdateAll(EffectMap.Models);

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

        ScriptViewModel observable = ScriptMap.Wrap(model);

        ScriptRootNode node = new()
        {
            Observable = observable,
            DeleteScriptCommand = DeleteScriptCommand,
        };

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

        RootNode.ScriptNodes.RemoveAll(node => node.Observable == observable);
    }
    private bool CanDeleteScript(object? param)
    {
        return param is ScriptViewModel;
    }

    #endregion
}

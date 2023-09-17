using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Messages;
using Paradoxical.Model.Entities;
using Paradoxical.Services;
using Paradoxical.Services.Entities;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
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
    public IPortraitService PortraitService { get; }
    public IOptionService OptionService { get; }
    public IOnionService OnionService { get; }
    public IDecisionService DecisionService { get; }
    public ILinkService LinkService { get; }

    private ModViewModel ModViewModel { get; set; } = new();

    private ModelMap<Script, ScriptViewModel> ScriptModelMap { get; set; } = new();
    private ModelMap<Event, EventViewModel> EventModelMap { get; set; } = new();
    private ModelMap<Option, OptionViewModel> OptionModelMap { get; set; } = new();
    private ModelMap<Onion, OnionViewModel> OnionModelMap { get; set; } = new();
    private ModelMap<Portrait, PortraitViewModel> PortraitModelMap { get; set; } = new();
    private ModelMap<Decision, DecisionViewModel> DecisionModelMap { get; set; } = new();
    private ModelMap<Link, LinkViewModel> LinkModelMap { get; set; } = new();

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
        IPortraitService portraitService,
        IOptionService optionService,
        IOnionService onionService,
        IDecisionService decisionService,
        ILinkService linkService)
        : base(shell, mediator)
    {
        DataService = dataService;

        ModService = modService;

        ScriptService = scriptService;
        EventService = eventService;
        PortraitService = portraitService;
        OptionService = optionService;
        OnionService = onionService;
        DecisionService = decisionService;
        LinkService = linkService;
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
        Selected = null;

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
        LinkModelMap = new(LinkService.Get());

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
        if (param is not IObservableNode node)
        { return; }

        var observable = node.Observable;
        Selected = observable;
    }
    private bool CanEditNode(object? param)
    {
        return param is IObservableNode;
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        FinderViewModel finder = new();

        finder.Items = Enumerable.Empty<IElementWrapper>()
            .Union(ScriptModelMap.Wrappers)
            .Union(EventModelMap.Wrappers)
            .Union(OptionModelMap.Wrappers)
            .Union(DecisionModelMap.Wrappers);

        await finder.Show();

        if (finder.DialogResult != true)
        { return; }

        if (finder.Selected == null)
        { return; }

        if (finder.Selected is not ObservableObject observable)
        { return; }

        Selected = observable;

        var node = ModNode.Descendants
            .OfType<IObservableNode>()
            .First(node => node.Observable == observable);

        node.Focus();
    }


    #region Node Init

    private void InitModNode(ModNode node)
    {
        node.EditCommand = EditCommand;

        node.CreateScriptCommand = CreateScriptCommand;
        node.CreateEventCommand = CreateEventCommand;
        node.CreateDecisionCommand = CreateDecisionCommand;
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

        node.LinkCommand = CreateOptionLinkCommand;
    }

    private void InitOptionBranch(OptionBranch node)
    {
        Option model = node.Observable.Model;

        foreach (var relation in OptionService.GetLinks(model))
        {
            var observable = LinkModelMap[relation];
            LinkNode child = new() { Observable = observable };

            InitLinkNode(child);

            node.LinkNodes.Add(child);
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
    }

    private void InitDecisionBranch(DecisionBranch node)
    {
    }

    private void InitLinkNode(LinkNode node)
    {
        node.DeleteCommand = DeleteLinkCommand;
        node.EditCommand = EditCommand;

        Link model = node.Observable.Model;
        Event relation = LinkService.GetEvent(model);

        var observable = EventModelMap[relation];
        EventLeaf child = new() { Observable = observable };

        InitEventNode(child);

        node.EventNode = child;
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

        InitScriptNode(node);
        InitScriptBranch(node);

        ModNode.ScriptNodes.Add(node);

        node.Select();
        node.Expand();
        node.CollapseSiblings();
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

        InitEventNode(node);
        InitEventBranch(node);

        ModNode.EventNodes.Add(node);

        node.Select();
        node.Expand();
        node.CollapseSiblings();
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
            var node = new OptionBranch() { Observable = relationViewModel };

            InitOptionNode(node);
            InitOptionBranch(node);

            parent.OptionNodes.Add(node);

            node.Select();
            node.Expand();
            node.CollapseSiblings();
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
            var node = new OnionBranch() { Observable = relationViewModel };

            InitOnionNode(node);
            InitOnionBranch(node);

            parent.OnionNodes.Add(node);

            node.Select();
            node.Expand();
            node.CollapseSiblings();
        }
    }
    private bool CanCreateEventOnion(object? param)
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

    private RelayCommand<object>? createOptionLinkCommand;
    public RelayCommand<object> CreateOptionLinkCommand => createOptionLinkCommand ??= new(CreateOptionLink, CanCreateOptionLink);

    private void CreateOptionLink(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        Link relation = new() { EventId = 1 }; // temporarily hardcoded
        LinkService.Insert(relation);

        var relationViewModel = LinkModelMap[relation];
        Selected = relationViewModel;

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            var node = new LinkNode() { Observable = relationViewModel };

            InitLinkNode(node);

            parent.LinkNodes.Add(node);

            node.Select();
            node.Expand();
            node.CollapseSiblings();
        }
    }
    private bool CanCreateOptionLink(object? param)
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

        InitDecisionNode(node);
        InitDecisionBranch(node);

        ModNode.DecisionNodes.Add(node);

        node.Select();
        node.Expand();
        node.CollapseSiblings();
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

    #endregion


    #region Link Commands

    private RelayCommand<object>? deleteLinkCommand;
    public RelayCommand<object> DeleteLinkCommand => deleteLinkCommand ??= new(DeleteLink, CanDeleteLink);

    private void DeleteLink(object? param)
    {
        if (param is not LinkViewModel observable)
        { return; }

        if (Selected is LinkViewModel selected && selected == observable)
        {
            Selected = null;
        }

        var model = observable.Model;

        LinkModelMap.Remove(model);
        LinkService.Delete(model);

        var nodes = RootNode.Descendants
            .OfType<LinkNode>()
            .Where(node => node.Observable == observable)
            .ToArray();

        var collections = RootNode.Descendants.OfType<CollectionNode>().ToArray();

        foreach (var collection in collections)
        {
            collection.RemoveAll(node => nodes.Contains(node));
        }
    }
    private bool CanDeleteLink(object? param)
    {
        return param is LinkViewModel;
    }

    #endregion
}

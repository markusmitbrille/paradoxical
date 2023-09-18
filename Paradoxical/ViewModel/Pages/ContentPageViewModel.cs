﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Entities;
using Paradoxical.Services;
using Paradoxical.Services.Entities;
using System;
using System.Linq;

namespace Paradoxical.ViewModel;

public class ContentPageViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Content";

    public IDataService DataService { get; }
    public IUpdateService UpdateService { get; }

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
        set
        {
            Update();
            SetProperty(ref selected, value);
    }
    }

    public ContentPageViewModel(
        IShell shell,
        IMediatorService mediator,
        IDataService dataService,
        IUpdateService updateService,
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
        UpdateService = updateService;

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

    private void Update()
    {
        if (selected is not IModelWrapper wrapper)
        { return; }

        if (wrapper.Model is not IEntity entity)
        { return; }

        UpdateService.Update(entity);
    }

    private void UpdateAll()
    {
        ModService.Update(ModViewModel.Model);

        ScriptService.UpdateAll(ScriptModelMap.Models);
        EventService.UpdateAll(EventModelMap.Models);
        OptionService.UpdateAll(OptionModelMap.Models);
        OnionService.UpdateAll(OnionModelMap.Models);
        PortraitService.UpdateAll(PortraitModelMap.Models);
        DecisionService.UpdateAll(DecisionModelMap.Models);
        LinkService.UpdateAll(LinkModelMap.Models);
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
        Selected = ModViewModel;

        ScriptModelMap = new(ScriptService.Get());
        EventModelMap = new(EventService.Get());
        OptionModelMap = new(OptionService.Get());
        OnionModelMap = new(OnionService.Get());
        PortraitModelMap = new(PortraitService.Get());
        DecisionModelMap = new(DecisionService.Get());
        LinkModelMap = new(LinkService.Get());

        ModNode = new()
        {
            Observable = ModViewModel,
            IsSelected = true,
            IsExpanded = true,
        };

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
        UpdateAll();

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

    private RelayCommand? findCommand;
    public RelayCommand FindCommand => findCommand ??= new(Find);

    private void Find()
    {
        FinderViewModel finder = new();

        finder.Items = Enumerable.Empty<IElementWrapper>()
            .Union(ScriptModelMap.Wrappers)
            .Union(EventModelMap.Wrappers)
            .Union(OptionModelMap.Wrappers)
            .Union(DecisionModelMap.Wrappers);

        var res = finder.Show();

        if (res != true)
        { return; }

        if (finder.Selected == null)
        { return; }

        if (finder.Selected is not ObservableObject observable)
        { return; }

        Selected = observable;

        ModNode.Descendants
            .OfType<IObservableNode>()
            .First(node => node.Observable == observable)
            .Focus();
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
        foreach (var relation in ScriptModelMap.Models)
        {
            CreateScriptBranch(relation, node.ScriptNodes);
        }

        foreach (var relation in EventModelMap.Models)
        {
            CreateEventBranch(relation, node.EventNodes);
        }

        foreach (var relation in DecisionModelMap.Models)
        {
            CreateDecisionBranch(relation, node.DecisionNodes);
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
            CreatePortraitBranch(relation, node.PortraitNodes);
        }

        foreach (var relation in EventService.GetOptions(model))
        {
            CreateOptionBranch(relation, node.OptionNodes);
        }

        foreach (var relation in EventService.GetOnions(model))
        {
            CreateOnionBranch(relation, node.OnionNodes);
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
        node.CreateEventCommand = CreateOptionEventCommand;
    }

    private void InitOptionBranch(OptionBranch node)
    {
        Option model = node.Observable.Model;

        foreach (var relation in OptionService.GetLinks(model))
        {
            CreateLinkNode(relation, node.LinkNodes);
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

        node.LinkCommand = CreateDecisionLinkCommand;
        node.CreateEventCommand = CreateDecisionEventCommand;
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

        node.EventNode = CreateEventLeaf(relation);
    }

    #endregion


    #region Node Create

    private ScriptBranch CreateScriptBranch(Script model, CollectionNode? collection = null)
    {
        var observable = ScriptModelMap[model];
        var node = new ScriptBranch() { Observable = observable };

        InitScriptNode(node);
        InitScriptBranch(node);

        collection?.Add(node);
        return node;
    }

    private ScriptLeaf CreateScriptLeaf(Script model, CollectionNode? collection = null)
    {
        var observable = ScriptModelMap[model];
        var node = new ScriptLeaf() { Observable = observable };

        InitScriptNode(node);

        collection?.Add(node);
        return node;
    }

    private EventBranch CreateEventBranch(Event model, CollectionNode? collection = null)
    {
        var observable = EventModelMap[model];
        var node = new EventBranch() { Observable = observable };

        InitEventNode(node);
        InitEventBranch(node);

        collection?.Add(node);
        return node;
    }

    private EventLeaf CreateEventLeaf(Event model, CollectionNode? collection = null)
    {
        var observable = EventModelMap[model];
        var node = new EventLeaf() { Observable = observable };

        InitEventNode(node);

        collection?.Add(node);
        return node;
    }

    private PortraitBranch CreatePortraitBranch(Portrait model, CollectionNode? collection = null)
    {
        var observable = PortraitModelMap[model];
        var node = new PortraitBranch() { Observable = observable };

        InitPortraitNode(node);
        InitPortraitBranch(node);

        collection?.Add(node);
        return node;
    }

    private PortraitLeaf CreatePortraitLeaf(Portrait model, CollectionNode? collection = null)
    {
        var observable = PortraitModelMap[model];
        var node = new PortraitLeaf() { Observable = observable };

        InitPortraitNode(node);

        collection?.Add(node);
        return node;
    }

    private OptionBranch CreateOptionBranch(Option model, CollectionNode? collection = null)
    {
        var observable = OptionModelMap[model];
        var node = new OptionBranch() { Observable = observable };

        InitOptionNode(node);
        InitOptionBranch(node);

        collection?.Add(node);
        return node;
    }

    private OptionLeaf CreateOptionLeaf(Option model, CollectionNode? collection = null)
    {
        var observable = OptionModelMap[model];
        var node = new OptionLeaf() { Observable = observable };

        InitOptionNode(node);

        collection?.Add(node);
        return node;
    }

    private OnionBranch CreateOnionBranch(Onion model, CollectionNode? collection = null)
    {
        var observable = OnionModelMap[model];
        var node = new OnionBranch() { Observable = observable };

        InitOnionNode(node);
        InitOnionBranch(node);

        collection?.Add(node);
        return node;
    }

    private OnionLeaf CreateOnionLeaf(Onion model, CollectionNode? collection = null)
    {
        var observable = OnionModelMap[model];
        var node = new OnionLeaf() { Observable = observable };

        InitOnionNode(node);

        collection?.Add(node);
        return node;
    }

    private DecisionBranch CreateDecisionBranch(Decision model, CollectionNode? collection = null)
    {
        var observable = DecisionModelMap[model];
        var node = new DecisionBranch() { Observable = observable };

        InitDecisionNode(node);
        InitDecisionBranch(node);

        collection?.Add(node);
        return node;
    }

    private DecisionLeaf CreateDecisionLeaf(Decision model, CollectionNode? collection = null)
    {
        var observable = DecisionModelMap[model];
        var node = new DecisionLeaf() { Observable = observable };

        InitDecisionNode(node);

        collection?.Add(node);
        return node;
    }

    private LinkNode CreateLinkNode(Link model, CollectionNode? collection = null)
    {
        var observable = LinkModelMap[model];
        var node = new LinkNode() { Observable = observable };

        InitLinkNode(node);

        collection?.Add(node);
        return node;
    }

    #endregion


    #region Script Commands

    private RelayCommand? createScriptCommand;
    public RelayCommand CreateScriptCommand => createScriptCommand ??= new(CreateScript);

    private void CreateScript()
    {
        var model = new Script();
        ScriptService.Insert(model);

        CreateScriptBranch(model, ModNode.ScriptNodes).Highlight();
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

        PortraitService.Insert(new() { EventId = model.Id, Position = PortraitPosition.Left });
        PortraitService.Insert(new() { EventId = model.Id, Position = PortraitPosition.Right });
        PortraitService.Insert(new() { EventId = model.Id, Position = PortraitPosition.LowerLeft });
        PortraitService.Insert(new() { EventId = model.Id, Position = PortraitPosition.LowerCenter });
        PortraitService.Insert(new() { EventId = model.Id, Position = PortraitPosition.LowerRight });

        CreateEventBranch(model, ModNode.EventNodes).Highlight();
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

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateOptionBranch(relation, parent.OptionNodes).Highlight();
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

        var parents = RootNode.Descendants
            .OfType<EventBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateOnionBranch(relation, parent.OptionNodes).Highlight();
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

        DeleteOptionLinks(model);

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

    private void DeleteOptionLinks(Option model)
    {
        var links = OptionService.GetLinks(model);
        foreach (var link in links)
        {
            var observable = LinkModelMap[link];
            DeleteLink(observable);
        }
    }

    private RelayCommand<object>? createOptionLinkCommand;
    public RelayCommand<object> CreateOptionLinkCommand => createOptionLinkCommand ??= new(CreateOptionLink, CanCreateOptionLink);

    private void CreateOptionLink(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        Option model = observable.Model;

        var events = EventService.Get();
        var links = OptionService.GetLinks(model);
        var unlinked = events.Where(evt => links.Any(link => link.EventId == evt.Id) == false);

        LinkerViewModel linker = new();
        linker.Items = unlinked.Select(evt => EventModelMap[evt]).ToList();

        var res = linker.Show();

        if (res != true)
        { return; }

        if (linker.Selected == null)
        { return; }

        Link relation = new()
        {
            EventId = linker.Selected.Id,

            Scope = linker.Scope,
            MinDays = linker.MinDays,
            MaxDays = linker.MaxDays,
        };

        LinkService.Insert(relation);
        OptionService.AddLink(model, relation);

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateLinkNode(relation, parent.LinkNodes);
        }
    }
    private bool CanCreateOptionLink(object? param)
    {
        return param is OptionViewModel;
    }

    private RelayCommand<object>? createOptionEventCommand;
    public RelayCommand<object> CreateOptionEventCommand => createOptionEventCommand ??= new(CreateOptionEvent, CanCreateOptionEvent);

    private void CreateOptionEvent(object? param)
    {
        if (param is not OptionViewModel observable)
        { return; }

        Option model = observable.Model;

        Event evt = new();
        EventService.Insert(evt);

        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.Left });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.Right });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerLeft });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerCenter });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerRight });

        CreateEventBranch(evt, ModNode.EventNodes).Focus();

        Link link = new() { EventId = evt.Id };

        LinkService.Insert(link);
        OptionService.AddLink(model, link);

        var parents = RootNode.Descendants
            .OfType<OptionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateLinkNode(link, parent.LinkNodes).ExpandAncestors();
        }
    }
    private bool CanCreateOptionEvent(object? param)
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

        CreateDecisionBranch(model, ModNode.DecisionNodes).Highlight();
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

        DeleteDecisionLinks(model);

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

    private void DeleteDecisionLinks(Decision model)
    {
        var links = DecisionService.GetLinks(model);
        foreach (var link in links)
        {
            var observable = LinkModelMap[link];
            DeleteLink(observable);
        }
    }

    private RelayCommand<object>? createDecisionLinkCommand;
    public RelayCommand<object> CreateDecisionLinkCommand => createDecisionLinkCommand ??= new(CreateDecisionLink, CanCreateDecisionLink);

    private void CreateDecisionLink(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        Decision model = observable.Model;

        var events = EventService.Get();
        var links = DecisionService.GetLinks(model);
        var unlinked = events.Where(evt => links.Any(link => link.EventId == evt.Id) == false);

        LinkerViewModel linker = new();
        linker.Items = unlinked.Select(evt => EventModelMap[evt]).ToList();

        var res = linker.Show();

        if (res != true)
        { return; }

        if (linker.Selected == null)
        { return; }

        Link relation = new()
        {
            EventId = linker.Selected.Id,

            Scope = linker.Scope,
            MinDays = linker.MinDays,
            MaxDays = linker.MaxDays,
        };

        LinkService.Insert(relation);
        DecisionService.AddLink(model, relation);

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateLinkNode(relation, parent.LinkNodes);
        }
    }
    private bool CanCreateDecisionLink(object? param)
    {
        return param is DecisionViewModel;
    }

    private RelayCommand<object>? createDecisionEventCommand;
    public RelayCommand<object> CreateDecisionEventCommand => createDecisionEventCommand ??= new(CreateDecisionEvent, CanCreateDecisionEvent);

    private void CreateDecisionEvent(object? param)
    {
        if (param is not DecisionViewModel observable)
        { return; }

        Decision model = observable.Model;

        Event evt = new();
        EventService.Insert(evt);

        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.Left });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.Right });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerLeft });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerCenter });
        PortraitService.Insert(new() { EventId = evt.Id, Position = PortraitPosition.LowerRight });

        CreateEventBranch(evt, ModNode.EventNodes).Focus();

        Link link = new() { EventId = evt.Id };

        LinkService.Insert(link);
        DecisionService.AddLink(model, link);

        var parents = RootNode.Descendants
            .OfType<DecisionBranch>()
            .Where(parent => parent.Observable == observable)
            .ToArray();

        foreach (var parent in parents)
        {
            CreateLinkNode(link, parent.LinkNodes).ExpandAncestors();
        }
    }
    private bool CanCreateDecisionEvent(object? param)
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

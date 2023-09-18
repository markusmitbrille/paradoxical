using Paradoxical.Model.Entities;
using Paradoxical.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxical.Services;

public interface ICloneService
{
    Script Clone(Script entity);
    Event Clone(Event entity);
    Portrait Clone(Portrait entity);
    Option Clone(Option entity);
    Onion Clone(Onion entity);
    Decision Clone(Decision entity);
    Link Clone(Link entity);
}

public class CloneService : ICloneService
{
    public IScriptService ScriptService { get; }
    public IEventService EventService { get; }
    public IPortraitService PortraitService { get; }
    public IOptionService OptionService { get; }
    public IOnionService OnionService { get; }
    public IDecisionService DecisionService { get; }
    public ILinkService LinkService { get; }

    public CloneService(
        IScriptService scriptService,
        IEventService eventService,
        IPortraitService portraitService,
        IOptionService optionService,
        IOnionService onionService,
        IDecisionService decisionService,
        ILinkService linkService)
    {
        ScriptService = scriptService;
        EventService = eventService;
        PortraitService = portraitService;
        OptionService = optionService;
        OnionService = onionService;
        DecisionService = decisionService;
        LinkService = linkService;
    }


    #region Script

    public Script Clone(Script entity)
    {
        var clone = ScriptService.Clone(entity);
        return clone;
    }

    #endregion


    #region Event

    public Event Clone(Event entity)
    {
        var clone = EventService.Clone(entity);

        foreach (var relation in EventService.GetPortraits(entity))
        {
            CloneEventPortrait(clone, relation);
        }

        foreach (var relation in EventService.GetOptions(entity))
        {
            CloneEventOption(clone, relation);
        }

        foreach (var relation in EventService.GetOnions(entity))
        {
            CloneEventOnion(clone, relation);
        }

        return clone;
    }

    private void CloneEventPortrait(Event owner, Portrait relation)
    {
        var clone = Clone(relation);
        clone.EventId = owner.Id;

        PortraitService.Update(clone);
    }

    private void CloneEventOption(Event owner, Option relation)
    {
        var clone = Clone(relation);
        clone.EventId = owner.Id;

        OptionService.Update(clone);
    }

    private void CloneEventOnion(Event owner, Onion relation)
    {
        var clone = Clone(relation);
        clone.EventId = owner.Id;

        OnionService.Update(clone);
    }

    #endregion


    #region Portrait

    public Portrait Clone(Portrait entity)
    {
        var clone = PortraitService.Clone(entity);
        return clone;
    }

    #endregion


    #region Option

    public Option Clone(Option entity)
    {
        var clone = OptionService.Clone(entity);

        foreach (var relation in OptionService.GetLinks(entity))
        {
            CloneOptionLink(clone, relation);
        }

        return clone;
    }

    private void CloneOptionLink(Option owner, Link relation)
    {
        var clone = Clone(relation);
        OptionService.AddLink(owner, clone);
    }

    #endregion


    #region Onion

    public Onion Clone(Onion entity)
    {
        var clone = OnionService.Clone(entity);
        return clone;
    }

    #endregion


    #region Decision

    public Decision Clone(Decision entity)
    {
        var clone = DecisionService.Clone(entity);

        foreach (var relation in DecisionService.GetLinks(entity))
        {
            CloneDecisionLink(clone, relation);
        }

        return clone;
    }

    private void CloneDecisionLink(Decision owner, Link relation)
    {
        var clone = Clone(relation);
        DecisionService.AddLink(owner, clone);
    }

    #endregion


    #region Link

    public Link Clone(Link entity)
    {
        var clone = LinkService.Clone(entity);
        return clone;
    }

    #endregion
}

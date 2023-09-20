using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Entities;

public interface IEventService : IEntityService<Event>
{
    IEnumerable<Option> GetOptions(Event model);

    IEnumerable<Onion> GetOnions(Event model);

    IEnumerable<Portrait> GetPortraits(Event model);

    Portrait? GetLeftPortrait(Event model);
    Portrait? GetRightPortrait(Event model);
    Portrait? GetLowerLeftPortrait(Event model);
    Portrait? GetLowerCenterPortrait(Event model);
    Portrait? GetLowerRightPortrait(Event model);

    IEnumerable<Link> GetOwnedLinks(Event model);

    IEnumerable<Link> GetLinks(Event model);
    void AddLink(Event model, Link relation);
    void RemoveLink(Event model, Link relation);
}

public class EventService : EntityService<Event>, IEventService
{
    public EventService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public override void Delete(Event model)
    {
        base.Delete(model);

        string deleteLinks = ParadoxQuery.CollectionDelete(
            mn: "event_links",
            fk: "event_id");

        Data.Connection.Execute(deleteLinks, model.Id);
    }

    public IEnumerable<Option> GetOptions(Event model)
    {
        string query = ParadoxQuery.Composition(
            c: "options",
            o: "events",
            fk: "event_id",
            pk: "id");

        return Data.Connection.Query<Option>(query, model.Id);
    }

    public IEnumerable<Onion> GetOnions(Event model)
    {
        string query = ParadoxQuery.Composition(
            c: "on_actions",
            o: "events",
            fk: "event_id",
            pk: "id");

        return Data.Connection.Query<Onion>(query, model.Id);
    }

    public IEnumerable<Portrait> GetPortraits(Event model)
    {
        string query = ParadoxQuery.Composition(
            c: "portraits",
            o: "events",
            fk: "event_id",
            pk: "id");

        return Data.Connection.Query<Portrait>(query, model.Id);
    }

    public Portrait? GetLeftPortrait(Event model)
    {
        return GetPortraits(model).FirstOrDefault(portrait => portrait.Position == PortraitPosition.Left);
    }

    public Portrait? GetRightPortrait(Event model)
    {
        return GetPortraits(model).FirstOrDefault(portrait => portrait.Position == PortraitPosition.Right);
    }

    public Portrait? GetLowerLeftPortrait(Event model)
    {
        return GetPortraits(model).FirstOrDefault(portrait => portrait.Position == PortraitPosition.LowerLeft);
    }

    public Portrait? GetLowerCenterPortrait(Event model)
    {
        return GetPortraits(model).FirstOrDefault(portrait => portrait.Position == PortraitPosition.LowerCenter);
    }

    public Portrait? GetLowerRightPortrait(Event model)
    {
        return GetPortraits(model).FirstOrDefault(portrait => portrait.Position == PortraitPosition.LowerRight);
    }

    public IEnumerable<Link> GetOwnedLinks(Event model)
    {
        string query = ParadoxQuery.Composition(
            c: "links",
            o: "events",
            fk: "event_id",
            pk: "id");

        return Data.Connection.Query<Link>(query, model.Id);
    }

    public IEnumerable<Link> GetLinks(Event model)
    {
        string query = ParadoxQuery.Collection(
            m: "links",
            n: "events",
            mn: "event_links",
            mfk: "link_id",
            nfk: "event_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Link>(query, model.Id);
    }
    public void AddLink(Event model, Link relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "event_links",
            mfk: "event_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveLink(Event model, Link relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "event_links",
            mfk: "event_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

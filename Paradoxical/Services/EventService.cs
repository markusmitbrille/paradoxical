using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEventService
{
    Event Get(int id);
    Event Get(Event model);

    IEnumerable<Event> Get();

    void Insert(Event model);
    void Delete(Event model);

    void Update(Event model);
    void UpdateAll(IEnumerable<Event> model);

    IEnumerable<Trigger> GetTriggers(Event model);
    void AddTrigger(EventTrigger relation);
    void RemoveTrigger(EventTrigger relation);

    IEnumerable<Effect> GetImmediateEffects(Event model);
    void SetImmediateEffects(Event model, IEnumerable<Effect> relations);

    IEnumerable<Effect> GetAfterEffects(Event model);
    void SetAfterEffects(Event model, IEnumerable<Effect> relations);

    IEnumerable<Option> GetOptions(Event model);
    void SetOptions(Event model, IEnumerable<Effect> relations);
}

public class EventService : IEventService
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    public EventService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public Event Get(int id)
    {
        return Data.Connection.Get<Event>(id);
    }
    public Event Get(Event model)
    {
        return Get(model.Id);
    }

    public IEnumerable<Event> Get()
    {
        return Data.Connection.Table<Event>().ToArray();
    }

    public void Insert(Event model)
    {
        Data.Connection.Insert(model);

        Mediator.Send<ElementInsertedMessage>(new(model));
        Mediator.Send<ElementSelectedMessage>(new(model));
    }
    public void Delete(Event model)
    {
        Data.Connection.Delete(model);

        Mediator.Send<ElementDeletedMessage>(new(model));
        Mediator.Send<ElementDeselectedMessage>(new(model));
    }

    public void Update(Event model)
    {
        Data.Connection.Update(model);
    }
    public void UpdateAll(IEnumerable<Event> models)
    {
        Data.Connection.UpdateAll(models);
    }

    public IEnumerable<Trigger> GetTriggers(Event model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "events",
            mn: "event_triggers",
            mfk: "trigger_id",
            nfk: "event_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddTrigger(EventTrigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "event_triggers",
            mfk: "trigger_id",
            nfk: "event_id");

        Data.Connection.Execute(query, relation.TriggerId, relation.EventId);

        Mediator.Send<RelationAddedMessage>(new(relation));
    }
    public void RemoveTrigger(EventTrigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "event_triggers",
            mfk: "trigger_id",
            nfk: "event_id");

        Data.Connection.Execute(query, relation.TriggerId, relation.EventId);

        Mediator.Send<RelationRemovedMessage>(new(relation));
    }

    public IEnumerable<Effect> GetImmediateEffects(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetImmediateEffects(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetAfterEffects(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetAfterEffects(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Option> GetOptions(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetOptions(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

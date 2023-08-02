using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IEventService : IEntityService<Event>
{
    IEnumerable<Option> GetOptions(Event model);

    IEnumerable<Onion> GetOnions(Event model);

    IEnumerable<Portrait> GetPortraits(Event model);

    Portrait GetLeftPortrait(Event model);
    Portrait GetRightPortrait(Event model);
    Portrait GetLowerLeftPortrait(Event model);
    Portrait GetLowerCenterPortrait(Event model);
    Portrait GetLowerRightPortrait(Event model);

    IEnumerable<Trigger> GetTriggers(Event model);
    void AddTrigger(Event model, Trigger relation);
    void RemoveTrigger(Event model, Trigger relation);

    IEnumerable<Effect> GetImmediates(Event model);
    void AddImmediate(Event model, Effect relation);
    void RemoveImmediate(Event model, Effect relation);

    IEnumerable<Effect> GetAfters(Event model);
    void AddAfter(Event model, Effect relation);
    void RemoveAfter(Event model, Effect relation);
}

public class EventService : EntityService<Event>, IEventService
{
    public EventService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public override void Delete(Event model)
    {
        base.Delete(model);

        string deleteOptions = ParadoxQuery.CompositionDelete(
            c: "options",
            fk: "event_id");

        string deletePortraits = ParadoxQuery.CompositionDelete(
            c: "portraits",
            fk: "event_id");

        Data.Connection.Execute(deleteOptions, model.Id);
        Data.Connection.Execute(deletePortraits, model.Id);

        string deleteEventTriggers = ParadoxQuery.CollectionDelete(
            mn: "event_triggers",
            fk: "event_id");

        string deleteEventImmediateEffects = ParadoxQuery.CollectionDelete(
            mn: "event_immediate_effects",
            fk: "event_id");

        string deleteEventAfterEffects = ParadoxQuery.CollectionDelete(
            mn: "event_after_effects",
            fk: "event_id");

        Data.Connection.Execute(deleteEventTriggers, model.Id);
        Data.Connection.Execute(deleteEventImmediateEffects, model.Id);
        Data.Connection.Execute(deleteEventAfterEffects, model.Id);
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

    public Portrait GetLeftPortrait(Event model)
    {
        return GetPortraits(model).Single(portrait => portrait.Position == PortraitPosition.Left);
    }

    public Portrait GetRightPortrait(Event model)
    {
        return GetPortraits(model).Single(portrait => portrait.Position == PortraitPosition.Right);
    }

    public Portrait GetLowerLeftPortrait(Event model)
    {
        return GetPortraits(model).Single(portrait => portrait.Position == PortraitPosition.LowerLeft);
    }

    public Portrait GetLowerCenterPortrait(Event model)
    {
        return GetPortraits(model).Single(portrait => portrait.Position == PortraitPosition.LowerCenter);
    }

    public Portrait GetLowerRightPortrait(Event model)
    {
        return GetPortraits(model).Single(portrait => portrait.Position == PortraitPosition.LowerRight);
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
    public void AddTrigger(Event model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "event_triggers",
            mfk: "event_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveTrigger(Event model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "event_triggers",
            mfk: "event_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Effect> GetImmediates(Event model)
    {
        string query = ParadoxQuery.Collection(
            m: "effects",
            n: "events",
            mn: "event_immediate_effects",
            mfk: "effect_id",
            nfk: "event_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Effect>(query, model.Id);
    }
    public void AddImmediate(Event model, Effect relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "event_immediate_effects",
            mfk: "event_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveImmediate(Event model, Effect relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "event_immediate_effects",
            mfk: "event_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Effect> GetAfters(Event model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "events",
            mn: "event_after_effects",
            mfk: "effect_id",
            nfk: "event_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Effect>(query, model.Id);
    }
    public void AddAfter(Event model, Effect relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "event_after_effects",
            mfk: "event_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveAfter(Event model, Effect relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "event_after_effects",
            mfk: "event_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

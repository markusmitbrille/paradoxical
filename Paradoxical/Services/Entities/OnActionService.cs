using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;

namespace Paradoxical.Services.Elements;

public interface IOnActionService : IEntityService<OnAction>
{
    IEnumerable<OnAction> GetChildren(OnAction model);
    void AddChild(OnAction model, OnAction relation);
    void RemoveChild(OnAction model, OnAction relation);

    IEnumerable<Event> GetRandoms(OnAction model);
    void AddRandom(OnAction model, Event relation);
    void RemoveRandom(OnAction model, Event relation);

    IEnumerable<Trigger> GetTriggers(OnAction model);
    void AddTrigger(OnAction model, Trigger relation);
    void RemoveTrigger(OnAction model, Trigger relation);

    IEnumerable<Effect> GetEffects(OnAction model);
    void AddEffect(OnAction model, Effect relation);
    void RemoveEffect(OnAction model, Effect relation);
}

public class OnActionService : EntityService<OnAction>, IOnActionService
{
    public OnActionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public IEnumerable<OnAction> GetChildren(OnAction model)
    {
        string query = ParadoxQuery.Collection(
            m: "on_actions",
            n: "on_actions",
            mn: "on_action_children",
            mfk: "child_id",
            nfk: "parent_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<OnAction>(query, model.Id);
    }
    public void AddChild(OnAction model, OnAction relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "on_action_children",
            mfk: "parent_id",
            nfk: "child_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveChild(OnAction model, OnAction relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "on_action_children",
            mfk: "parent_id",
            nfk: "child_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Event> GetRandoms(OnAction model)
    {
        string query = ParadoxQuery.Collection(
            m: "events",
            n: "on_actions",
            mn: "on_action_random_events",
            mfk: "event_id",
            nfk: "on_action_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Event>(query, model.Id);
    }
    public void AddRandom(OnAction model, Event relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "on_action_random_events",
            mfk: "on_action_id",
            nfk: "event_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveRandom(OnAction model, Event relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "on_action_random_events",
            mfk: "on_action_id",
            nfk: "event_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Trigger> GetTriggers(OnAction model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "on_actions",
            mn: "on_action_triggers",
            mfk: "trigger_id",
            nfk: "on_action_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddTrigger(OnAction model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "on_action_triggers",
            mfk: "on_action_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveTrigger(OnAction model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "on_action_triggers",
            mfk: "on_action_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Effect> GetEffects(OnAction model)
    {
        string query = ParadoxQuery.Collection(
            m: "effects",
            n: "on_actions",
            mn: "on_action_effects",
            mfk: "effect_id",
            nfk: "on_action_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Effect>(query, model.Id);
    }
    public void AddEffect(OnAction model, Effect relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "on_action_effects",
            mfk: "on_action_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveEffect(OnAction model, Effect relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "on_action_effects",
            mfk: "on_action_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

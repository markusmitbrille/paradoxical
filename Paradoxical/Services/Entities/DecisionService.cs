using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IDecisionService : IEntityService<Decision>
{
    Event? GetTriggeredEvent(Decision model);

    IEnumerable<Trigger> GetShowns(Decision model);
    void AddShown(Decision model, Trigger relation);
    void RemoveShown(Decision model, Trigger relation);

    IEnumerable<Trigger> GetValids(Decision model);
    void AddValid(Decision model, Trigger relation);
    void RemoveValid(Decision model, Trigger relation);

    IEnumerable<Trigger> GetFailures(Decision model);
    void AddFailure(Decision model, Trigger relation);
    void RemoveFailure(Decision model, Trigger relation);

    IEnumerable<Effect> GetEffects(Decision model);
    void AddEffect(Decision model, Effect relation);
    void RemoveEffect(Decision model, Effect relation);
}

public class DecisionService : EntityService<Decision>, IDecisionService
{
    public DecisionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public Event? GetTriggeredEvent(Decision model)
    {
        string query = ParadoxQuery.Composite(
            c: "decisions",
            o: "events",
            fk: "triggered_event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).SingleOrDefault();
    }

    public IEnumerable<Trigger> GetShowns(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_is_shown_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddShown(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_is_shown_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveShown(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_is_shown_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Trigger> GetValids(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_is_valid_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddValid(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_is_valid_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveValid(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_is_valid_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Trigger> GetFailures(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_is_valid_failure_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddFailure(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_is_valid_failure_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveFailure(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_is_valid_failure_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Effect> GetEffects(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "effects",
            n: "decisions",
            mn: "decision_effects",
            mfk: "effect_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Effect>(query, model.Id);
    }
    public void AddEffect(Decision model, Effect relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_effects",
            mfk: "decision_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveEffect(Decision model, Effect relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_effects",
            mfk: "decision_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

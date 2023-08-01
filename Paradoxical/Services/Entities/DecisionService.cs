using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IDecisionService : IEntityService<Decision>
{
    Event? GetTriggeredEvent(Decision model);

    IEnumerable<Trigger> GetShownTriggers(Decision model);
    void AddShownTrigger(Decision model, Trigger relation);
    void RemoveShownTrigger(Decision model, Trigger relation);

    IEnumerable<Trigger> GetFailureTriggers(Decision model);
    void AddFailureTrigger(Decision model, Trigger relation);
    void RemoveFailureTrigger(Decision model, Trigger relation);

    IEnumerable<Trigger> GetValidTriggers(Decision model);
    void AddValidTrigger(Decision model, Trigger relation);
    void RemoveValidTrigger(Decision model, Trigger relation);

    IEnumerable<Effect> GetEffects(Decision model);
    void AddEffect(Decision model, Effect relation);
    void RemoveEffect(Decision model, Effect relation);

    IEnumerable<Trigger> GetAiPotentialTriggers(Decision model);
    void AddAiPotentialTrigger(Decision model, Trigger relation);
    void RemoveAiPotentialTrigger(Decision model, Trigger relation);
}

public class DecisionService : EntityService<Decision>, IDecisionService
{
    public DecisionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public override void Delete(Decision model)
    {
        base.Delete(model);

        string deleteDecisionShownTriggers = ParadoxQuery.CollectionDelete(
            mn: "decision_shown_triggers",
            fk: "decision_id");

        string deleteDecisionFailureTriggers = ParadoxQuery.CollectionDelete(
            mn: "decision_failure_triggers",
            fk: "decision_id");

        string deleteDecisionValidTriggers = ParadoxQuery.CollectionDelete(
            mn: "decision_valid_triggers",
            fk: "decision_id");

        string deleteDecisionEffects = ParadoxQuery.CollectionDelete(
            mn: "decision_effects",
            fk: "decision_id");

        string deleteDecisionAiPotentialTriggers = ParadoxQuery.CollectionDelete(
            mn: "decision_ai_potential_triggers",
            fk: "decision_id");

        Data.Connection.Execute(deleteDecisionShownTriggers, model.Id);
        Data.Connection.Execute(deleteDecisionFailureTriggers, model.Id);
        Data.Connection.Execute(deleteDecisionValidTriggers, model.Id);
        Data.Connection.Execute(deleteDecisionEffects, model.Id);
        Data.Connection.Execute(deleteDecisionAiPotentialTriggers, model.Id);
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

    public IEnumerable<Trigger> GetShownTriggers(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_shown_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddShownTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_shown_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveShownTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_shown_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Trigger> GetFailureTriggers(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_failure_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddFailureTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_failure_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveFailureTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_failure_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Trigger> GetValidTriggers(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_valid_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddValidTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_valid_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveValidTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_valid_triggers",
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

    public IEnumerable<Trigger> GetAiPotentialTriggers(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "decisions",
            mn: "decision_ai_potential_triggers",
            mfk: "trigger_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddAiPotentialTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_ai_potential_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveAiPotentialTrigger(Decision model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_ai_potential_triggers",
            mfk: "decision_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

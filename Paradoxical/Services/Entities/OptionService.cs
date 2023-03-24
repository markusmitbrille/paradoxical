using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IOptionService : IEntityService<Option>
{
    Event GetEvent(Option model);

    Event? GetTriggeredEvent(Option model);

    IEnumerable<Trigger> GetTriggers(Option model);
    void AddTrigger(Option model, Trigger relation);
    void RemoveTrigger(Option model, Trigger relation);

    IEnumerable<Effect> GetEffects(Option model);
    void AddEffect(Option model, Effect relation);
    void RemoveEffect(Option model, Effect relation);
}

public class OptionService : EntityService<Option>, IOptionService
{
    public OptionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public Event GetEvent(Option model)
    {
        string query = ParadoxQuery.Composite(
            c: "options",
            o: "events",
            fk: "event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).Single();
    }

    public Event? GetTriggeredEvent(Option model)
    {
        string query = ParadoxQuery.Composite(
            c: "options",
            o: "events",
            fk: "triggered_event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).SingleOrDefault();
    }

    public IEnumerable<Trigger> GetTriggers(Option model)
    {
        string query = ParadoxQuery.Collection(
            m: "triggers",
            n: "options",
            mn: "option_triggers",
            mfk: "trigger_id",
            nfk: "option_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Trigger>(query, model.Id);
    }
    public void AddTrigger(Option model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "option_triggers",
            mfk: "option_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveTrigger(Option model, Trigger relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "option_triggers",
            mfk: "option_id",
            nfk: "trigger_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }

    public IEnumerable<Effect> GetEffects(Option model)
    {
        string query = ParadoxQuery.Collection(
            m: "effects",
            n: "options",
            mn: "option_effects",
            mfk: "effect_id",
            nfk: "option_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Effect>(query, model.Id);
    }
    public void AddEffect(Option model, Effect relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "option_effects",
            mfk: "option_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveEffect(Option model, Effect relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "option_effects",
            mfk: "option_id",
            nfk: "effect_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

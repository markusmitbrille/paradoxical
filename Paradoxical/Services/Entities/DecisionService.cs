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
}

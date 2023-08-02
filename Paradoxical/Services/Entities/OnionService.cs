using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IOnionService : IEntityService<Onion>
{
    Event GetEvent(Onion model);
}

public class OnionService : EntityService<Onion>, IOnionService
{
    public OnionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public Event GetEvent(Onion model)
    {
        string query = ParadoxQuery.Composite(
            c: "on_actions",
            o: "events",
            fk: "event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).Single();
    }
}

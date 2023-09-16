using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Entities;

public interface ILinkService : IEntityService<Link>
{
    Event GetEvent(Link model);
}

public class LinkService : EntityService<Link>, ILinkService
{
    public LinkService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public Event GetEvent(Link model)
    {
        string query = ParadoxQuery.Composite(
            c: "links",
            o: "events",
            fk: "event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).Single();
    }
}

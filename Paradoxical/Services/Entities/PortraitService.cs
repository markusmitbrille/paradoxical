using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IPortraitService : IEntityService<Portrait>
{
    Event GetEvent(Portrait model);
}

public class PortraitService : EntityService<Portrait>, IPortraitService
{
    public PortraitService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public Event GetEvent(Portrait model)
    {
        string query = ParadoxQuery.Composite(
            c: "portraits",
            o: "events",
            fk: "event_id",
            cpk: "id",
            opk: "id");

        return Data.Connection.Query<Event>(query, model.Id).Single();
    }
}

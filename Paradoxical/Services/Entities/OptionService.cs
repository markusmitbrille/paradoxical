using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IOptionService : IEntityService<Option>
{
    Event GetEvent(Option model);

    Event? GetTriggeredEvent(Option model);
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
}

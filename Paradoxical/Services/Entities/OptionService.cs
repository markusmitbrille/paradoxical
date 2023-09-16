using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Elements;

public interface IOptionService : IEntityService<Option>
{
    Event GetEvent(Option model);

    IEnumerable<Link> GetLinks(Option model);
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

    public IEnumerable<Link> GetLinks(Option model)
    {
        string query = ParadoxQuery.Collection(
            m: "links",
            n: "options",
            mn: "option_links",
            mfk: "link_id",
            nfk: "option_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Link>(query, model.Id);
    }
}

using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Paradoxical.Services.Entities;

public interface IOptionService : IEntityService<Option>
{
    Event GetEvent(Option model);

    IEnumerable<Link> GetLinks(Option model);
    void AddLink(Option model, Link relation);
    void RemoveLink(Option model, Link relation);
}

public class OptionService : EntityService<Option>, IOptionService
{
    public OptionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public override void Delete(Option model)
    {
        base.Delete(model);

        string deleteLinks = ParadoxQuery.CollectionDelete(
            mn: "option_links",
            fk: "option_id");

        Data.Connection.Execute(deleteLinks, model.Id);
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
    public void AddLink(Option model, Link relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "option_links",
            mfk: "option_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveLink(Option model, Link relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "option_links",
            mfk: "option_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

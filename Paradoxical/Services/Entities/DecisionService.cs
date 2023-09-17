using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services.Entities;

public interface IDecisionService : IEntityService<Decision>
{
    IEnumerable<Link> GetLinks(Decision model);
    void AddLink(Decision model, Link relation);
    void RemoveLink(Decision model, Link relation);
}

public class DecisionService : EntityService<Decision>, IDecisionService
{
    public DecisionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public override void Delete(Decision model)
    {
        base.Delete(model);

        string deleteLinks = ParadoxQuery.CollectionDelete(
            mn: "decision_links",
            fk: "decision_id");

        Data.Connection.Execute(deleteLinks, model.Id);
    }

    public IEnumerable<Link> GetLinks(Decision model)
    {
        string query = ParadoxQuery.Collection(
            m: "links",
            n: "decisions",
            mn: "decision_links",
            mfk: "link_id",
            nfk: "decision_id",
            mpk: "id",
            npk: "id");

        return Data.Connection.Query<Link>(query, model.Id);
    }
    public void AddLink(Decision model, Link relation)
    {
        string query = ParadoxQuery.CollectionAdd(
            mn: "decision_links",
            mfk: "decision_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
    public void RemoveLink(Decision model, Link relation)
    {
        string query = ParadoxQuery.CollectionRemove(
            mn: "decision_links",
            mfk: "decision_id",
            nfk: "link_id");

        Data.Connection.Execute(query, model.Id, relation.Id);
    }
}

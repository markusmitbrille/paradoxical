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
}

public class DecisionService : EntityService<Decision>, IDecisionService
{
    public DecisionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
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
}

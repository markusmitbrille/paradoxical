using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IDecisionValidService : IRelationshipService<Decision, Trigger, DecisionValid>
{
}

public class DecisionValidService : RelationshipService<Decision, Trigger, DecisionValid>, IDecisionValidService
{
    protected override string OwnerTable => "decisions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "decision_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "decision_is_valid_triggers";

    public DecisionValidService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

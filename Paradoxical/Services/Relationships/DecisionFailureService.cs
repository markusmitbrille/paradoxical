using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IDecisionFailureService : IRelationshipService<Decision, Trigger, DecisionFailure>
{
}

public class DecisionFailureService : RelationshipService<Decision, Trigger, DecisionFailure>, IDecisionFailureService
{
    protected override string OwnerTable => "decisions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "decision_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "decision_is_valid_failure_triggers";

    public DecisionFailureService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

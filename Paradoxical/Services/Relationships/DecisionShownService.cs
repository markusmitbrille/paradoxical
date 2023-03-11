using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IDecisionShownService : IRelationshipService<Decision, Trigger, DecisionShown>
{
}

public class DecisionShownService : RelationshipService<Decision, Trigger, DecisionShown>, IDecisionShownService
{
    protected override string OwnerTable => "decisions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "decision_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "decision_is_shown_triggers";

    public DecisionShownService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

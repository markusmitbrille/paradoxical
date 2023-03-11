using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IDecisionEffectService : IRelationshipService<Decision, Effect, DecisionEffect>
{
}

public class DecisionEffectService : RelationshipService<Decision, Effect, DecisionEffect>, IDecisionEffectService
{
    protected override string OwnerTable => "decisions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "decision_id";

    protected override string RelationTable => "effects";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "effect_id";

    protected override string RelationshipTable => "decision_effects";

    public DecisionEffectService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

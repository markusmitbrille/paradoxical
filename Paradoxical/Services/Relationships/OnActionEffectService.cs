using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOnActionEffectService : IRelationshipService<OnAction, Effect, OnActionEffect>
{
}

public class OnActionEffectService : RelationshipService<OnAction, Effect, OnActionEffect>, IOnActionEffectService
{
    protected override string OwnerTable => "on_actions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "on_action_id";

    protected override string RelationTable => "effects";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "effect_id";

    protected override string RelationshipTable => "on_action_effects";

    public OnActionEffectService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

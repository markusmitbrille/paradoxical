using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOnActionTriggerService : IRelationshipService<OnAction, Trigger, OnActionTrigger>
{
}

public class OnActionTriggerService : RelationshipService<OnAction, Trigger, OnActionTrigger>, IOnActionTriggerService
{
    protected override string OwnerTable => "on_actions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "on_action_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "on_action_triggers";

    public OnActionTriggerService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

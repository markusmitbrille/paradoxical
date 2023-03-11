using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOnActionOnActionService : IRelationshipService<OnAction, OnAction, OnActionOnAction>
{
}

public class OnActionOnActionService : RelationshipService<OnAction, OnAction, OnActionOnAction>, IOnActionOnActionService
{
    protected override string OwnerTable => "on_actions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "triggering_on_action_id";

    protected override string RelationTable => "on_actions";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "triggered_on_action_id";

    protected override string RelationshipTable => "on_action_on_actions";

    public OnActionOnActionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

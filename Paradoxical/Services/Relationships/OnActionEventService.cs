using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOnActionEventService : IRelationshipService<OnAction, Event, OnActionEvent>
{
}

public class OnActionEventService : RelationshipService<OnAction, Event, OnActionEvent>, IOnActionEventService
{
    protected override string OwnerTable => "on_actions";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "on_action_id";

    protected override string RelationTable => "events";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "event_id";

    protected override string RelationshipTable => "on_action_events";

    public OnActionEventService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IEventTriggerService : IRelationshipService<Event, Trigger, EventTrigger>
{
}

public class EventTriggerService : RelationshipService<Event, Trigger, EventTrigger>, IEventTriggerService
{
    protected override string OwnerTable => "events";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "event_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "event_triggers";

    public EventTriggerService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

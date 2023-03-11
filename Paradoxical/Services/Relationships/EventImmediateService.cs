using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IEventImmediateService : IRelationshipService<Event, Effect, EventImmediate>
{
}

public class EventImmediateService : RelationshipService<Event, Effect, EventImmediate>, IEventImmediateService
{
    protected override string OwnerTable => "events";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "event_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "event_immediate_effects";

    public EventImmediateService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

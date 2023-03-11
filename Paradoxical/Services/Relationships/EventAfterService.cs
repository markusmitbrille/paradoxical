using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IEventAfterService : IRelationshipService<Event, Effect, EventAfter>
{
}

public class EventAfterService : RelationshipService<Event, Effect, EventAfter>, IEventAfterService
{
    protected override string OwnerTable => "events";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "event_id";

    protected override string RelationTable => "effects";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "effect_id";

    protected override string RelationshipTable => "event_after_effects";

    public EventAfterService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

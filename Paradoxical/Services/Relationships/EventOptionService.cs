using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IEventOptionService : IRelationshipService<Event, Option, EventOption>
{
}

public class EventOptionService : RelationshipService<Event, Option, EventOption>, IEventOptionService
{
    protected override string OwnerTable => "events";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "event_id";

    protected override string RelationTable => "options";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "option_id";

    protected override string RelationshipTable => "event_options";

    public EventOptionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

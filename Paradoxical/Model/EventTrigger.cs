using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model;

[Table("event_triggers")]
public class EventTrigger : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }

    int IRelationship.OwnerID => EventId;
    int IRelationship.RelationID => TriggerId;
}

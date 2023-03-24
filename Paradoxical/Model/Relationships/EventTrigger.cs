using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("event_triggers")]
public class EventTrigger : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

using SQLite;

namespace Paradoxical.Model;

[Table("event_triggers")]
public class EventTrigger
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

using SQLite;

namespace Paradoxical.Model;

[Table("on_action_events")]
public class OnActionEvent
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("event_id"), Indexed]
    public int EventId { get; set; }
}

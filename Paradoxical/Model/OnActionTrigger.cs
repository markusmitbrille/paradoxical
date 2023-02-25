using SQLite;

namespace Paradoxical.Model;

[Table("on_action_triggers")]
public class OnActionTrigger
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

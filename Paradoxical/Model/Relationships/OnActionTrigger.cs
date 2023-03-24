using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_triggers")]
public class OnActionTrigger : IRelationship
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

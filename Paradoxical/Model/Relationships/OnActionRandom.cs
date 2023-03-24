using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_random_events")]
public class OnActionRandom : IRelationship
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("event_id"), Indexed]
    public int EventId { get; set; }
}

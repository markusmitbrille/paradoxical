using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_events")]
public class OnActionEvent : IRelationship
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    int IRelationship.OwnerID => OnActionId;
    int IRelationship.RelationID => EventId;
}

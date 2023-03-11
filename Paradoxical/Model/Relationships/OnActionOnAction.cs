using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_on_actions")]
public class OnActionOnAction : IRelationship
{
    [Column("triggering_on_action_id"), Indexed]
    public int TriggeringOnActionId { get; set; }

    [Column("triggered_on_action_id"), Indexed]
    public int TriggeredOnActionId { get; set; }

    int IRelationship.OwnerID => TriggeringOnActionId;
    int IRelationship.RelationID => TriggeredOnActionId;
}

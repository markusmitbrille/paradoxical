using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("option_triggers")]
public class OptionTrigger : IRelationship
{
    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }

    int IRelationship.OwnerID => OptionId;
    int IRelationship.RelationID => TriggerId;
}

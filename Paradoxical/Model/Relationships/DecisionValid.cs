using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("decision_is_valid_triggers")]
public class DecisionValid : IRelationship
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }

    int IRelationship.OwnerID => DecisionId;
    int IRelationship.RelationID => TriggerId;
}

using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("decision_valid_triggers")]
public class DecisionValidTrigger : IRelationship
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

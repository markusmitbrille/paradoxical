using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("decision_shown_triggers")]
public class DecisionShownTrigger : IRelationship
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

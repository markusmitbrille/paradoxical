using SQLite;

namespace Paradoxical.Model;

[Table("decision_is_valid_triggers")]
public class DecisionIsValidTrigger
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

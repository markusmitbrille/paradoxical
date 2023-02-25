using SQLite;

namespace Paradoxical.Model;

[Table("decision_is_valid_failure_triggers")]
public class DecisionIsValidFailureTrigger
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

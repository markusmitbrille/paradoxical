using SQLite;

namespace Paradoxical.Model;

[Table("decision_is_shown_triggers")]
public class DecisionIsShownTrigger
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

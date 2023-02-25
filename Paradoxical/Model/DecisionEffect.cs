using SQLite;

namespace Paradoxical.Model;

[Table("decision_effects")]
public class DecisionEffect
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

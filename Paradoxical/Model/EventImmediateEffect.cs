using SQLite;

namespace Paradoxical.Model;

[Table("event_immediate_effects")]
public class EventImmediateEffect
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

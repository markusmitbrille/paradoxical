using SQLite;

namespace Paradoxical.Model;

[Table("event_after_effects")]
public class EventAfterEffect
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

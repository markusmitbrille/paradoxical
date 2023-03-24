using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("event_after_effects")]
public class EventAfter : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

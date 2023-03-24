using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("event_immediate_effects")]
public class EventImmediate : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

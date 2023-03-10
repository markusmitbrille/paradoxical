using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model;

[Table("event_immediate_effects")]
public class EventImmediateEffect : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }

    int IRelationship.OwnerID => EventId;
    int IRelationship.RelationID => EffectId;
}

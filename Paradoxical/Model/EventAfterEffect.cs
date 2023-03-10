using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model;

[Table("event_after_effects")]
public class EventAfterEffect : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }

    int IRelationship.OwnerID => EventId;
    int IRelationship.RelationID => EffectId;
}

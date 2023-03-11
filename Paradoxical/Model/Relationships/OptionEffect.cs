using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("option_effects")]
public class OptionEffect : IRelationship
{
    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }

    int IRelationship.OwnerID => OptionId;
    int IRelationship.RelationID => EffectId;
}

using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("decision_effects")]
public class DecisionEffect : IRelationship
{
    [Column("decision_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }

    int IRelationship.OwnerID => DecisionId;
    int IRelationship.RelationID => EffectId;
}

using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_effects")]
public class OnActionEffect : IRelationship
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }

    int IRelationship.OwnerID => OnActionId;
    int IRelationship.RelationID => EffectId;
}

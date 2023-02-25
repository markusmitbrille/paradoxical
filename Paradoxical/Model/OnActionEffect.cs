using SQLite;

namespace Paradoxical.Model;

[Table("on_action_effects")]
public class OnActionEffect
{
    [Column("on_action_id"), Indexed]
    public int OnActionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

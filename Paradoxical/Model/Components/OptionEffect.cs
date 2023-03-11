using SQLite;

namespace Paradoxical.Model.Components;

[Table("option_effects")]
public class OptionEffect
{
    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    [Column("effect_id"), Indexed]
    public int EffectId { get; set; }
}

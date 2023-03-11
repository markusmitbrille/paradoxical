using SQLite;

namespace Paradoxical.Model.Components;

[Table("option_triggers")]
public class OptionTrigger
{
    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    [Column("trigger_id"), Indexed]
    public int TriggerId { get; set; }
}

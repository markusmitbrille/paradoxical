using SQLite;

namespace Paradoxical.Model;

[Table("on_action_on_actions")]
public class OnActionOnAction
{
    [Column("triggering_on_action_id"), Indexed]
    public int TriggeringOnActionId { get; set; }

    [Column("triggered_on_action_id"), Indexed]
    public int TriggeredOnActionId { get; set; }
}

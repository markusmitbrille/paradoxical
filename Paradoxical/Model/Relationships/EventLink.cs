using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("event_links")]
public class EventLink : IRelationship
{
    [Column("event_id"), Indexed]
    public int DecisionId { get; set; }

    [Column("link_id"), Indexed]
    public int LinkId { get; set; }
}

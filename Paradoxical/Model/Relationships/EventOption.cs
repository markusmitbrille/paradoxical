using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("event_options")]
public class EventOption : IRelationship
{
    [Column("event_id"), Indexed]
    public int EventId { get; set; }

    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    int IRelationship.OwnerID => EventId;
    int IRelationship.RelationID => OptionId;
}

using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("option_links")]
public class OptionLink : IRelationship
{
    [Column("option_id"), Indexed]
    public int OptionId { get; set; }

    [Column("link_id"), Indexed]
    public int LinkId { get; set; }
}

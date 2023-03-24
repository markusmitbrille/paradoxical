using Paradoxical.Core;
using SQLite;

namespace Paradoxical.Model.Relationships;

[Table("on_action_children")]
public class OnActionChild : IRelationship
{
    [Column("parent_id"), Indexed]
    public int ParentId { get; set; }

    [Column("child_id"), Indexed]
    public int ChildId { get; set; }
}

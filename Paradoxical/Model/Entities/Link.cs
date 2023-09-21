using Paradoxical.Core;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model.Entities;

[Table("links")]
public class Link : IEntity, IModel, IEquatable<Link?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

    [Column("scope")]
    public string Scope { get => scope; set => scope = value; }
    public string scope = "";

    [Column("min_days")]
    public int MinDays { get => minDays; set => minDays = value; }
    public int minDays;

    [Column("max_days")]
    public int MaxDays { get => maxDays; set => maxDays = value; }
    public int maxDays;

    public void Write(TextWriter writer, IModService modService, ILinkService linkService)
    {
        var evt = linkService.GetEvent(this);

        if (scope != string.Empty)
        {
            writer.Indent().WriteLine($"{scope} = {{");
            ParadoxText.IndentLevel++;
        }

        writer.Indent().WriteLine("trigger_event = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"id = {evt.GetQualifiedName(modService)}");
        writer.Indent().WriteLine($"days = {{ {minDays} {maxDays} }}");

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");

        if (scope != string.Empty)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Link);
    }

    public bool Equals(Link? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Link? left, Link? right)
    {
        return EqualityComparer<Link>.Default.Equals(left, right);
    }

    public static bool operator !=(Link? left, Link? right)
    {
        return !(left == right);
    }
}

using Paradoxical.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model;

[Table("portraits")]
public class Portrait : IComponent, IEquatable<Portrait?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed, NotNull]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

    int IComponent.OwnerId => EventId;

    [Column("position"), NotNull]
    public PortraitPosition Position { get => position; set => position = value; }
    public PortraitPosition position;

    [Column("character"), NotNull]
    public string Character { get => character; set => character = value; }
    public string character = "";

    [Column("animation"), NotNull]
    public string Animation { get => animation; set => animation = value; }
    public string animation = "";

    [Column("outfit"), NotNull]
    public string OutfitTags { get => outfitTags; set => outfitTags = value; }
    public string outfitTags = "";

    public void Write(TextWriter writer)
    {
        if (Character == string.Empty)
        {
            writer.Indent().WriteLine("character = ROOT");
        }
        else
        {
            writer.Indent().WriteLine($"character = {Character}");
        }

        if (Animation != string.Empty)
        {
            writer.Indent().WriteLine($"animation = {Animation}");
        }

        if (OutfitTags != string.Empty)
        {
            writer.Indent().WriteLine($"outfit_tags = {{ {OutfitTags} }}");
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Portrait);
    }

    public bool Equals(Portrait? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Portrait? left, Portrait? right)
    {
        return EqualityComparer<Portrait>.Default.Equals(left, right);
    }

    public static bool operator !=(Portrait? left, Portrait? right)
    {
        return !(left == right);
    }
}
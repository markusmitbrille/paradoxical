using Paradoxical.Core;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Animation;

namespace Paradoxical.Model.Entities;

public enum PortraitPosition
{
    None,
    Left,
    Right,
    LowerLeft,
    LowerCenter,
    LowerRight,
}

[Table("portraits")]
public class Portrait : IEntity, IModel, IEquatable<Portrait?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed, NotNull]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

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

    public void Write(
        TextWriter writer,
        IPortraitService portraitService)
    {
        if (Character == string.Empty)
        { return; }

        if (Position == PortraitPosition.Left)
        {
            writer.Indent().WriteLine("left_portrait = {");
            ParadoxText.IndentLevel++;
        }
        if (Position == PortraitPosition.Right)
        {
            writer.Indent().WriteLine("right_portrait = {");
            ParadoxText.IndentLevel++;
        }
        if (Position == PortraitPosition.LowerLeft)
        {
            writer.Indent().WriteLine("lower_left_portrait = {");
            ParadoxText.IndentLevel++;
        }
        if (Position == PortraitPosition.LowerCenter)
        {
            writer.Indent().WriteLine("lower_center_portrait = {");
            ParadoxText.IndentLevel++;
        }
        if (Position == PortraitPosition.LowerRight)
        {
            writer.Indent().WriteLine("lower_right_portrait = {");
            ParadoxText.IndentLevel++;
        }

        writer.Indent().WriteLine($"character = {Character}");

        if (Animation != string.Empty)
        {
            writer.Indent().WriteLine($"animation = {Animation}");
        }

        if (OutfitTags != string.Empty)
        {
            writer.Indent().WriteLine($"outfit_tags = {{ {OutfitTags} }}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
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

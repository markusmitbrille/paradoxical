using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Entities;

[Table("on_actions")]
public class Onion : IEntity, IModel, IElement, IEquatable<Onion?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed, NotNull]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = $"act_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("random"), NotNull]
    public bool Random { get => random; set => random = value; }
    public bool random;

    [Column("weight"), NotNull]
    public int Weight { get => weight; set => weight = value; }
    public int weight;

    public Onion()
    {
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Onion);
    }

    public bool Equals(Onion? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Onion? left, Onion? right)
    {
        return EqualityComparer<Onion>.Default.Equals(left, right);
    }

    public static bool operator !=(Onion? left, Onion? right)
    {
        return !(left == right);
    }
}
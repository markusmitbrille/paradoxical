using Paradoxical.Core;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model.Entities;

[Table("scripts")]
public class Script : IEntity, IModel, IEquatable<Script?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = $"src_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("code"), NotNull]
    public string Code { get => code; set => code = value; }
    public string code = "";

    [Column("dir"), NotNull]
    public string Dir { get => dir; set => dir = value; }
    public string dir = "./";

    [Column("file"), NotNull]
    public string File { get => file; set => file = value; }
    public string file = $"script.txt";

    public void Write(
        TextWriter writer)
    {
        writer.Write(Code);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Script);
    }

    public bool Equals(Script? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Script? left, Script? right)
    {
        return EqualityComparer<Script>.Default.Equals(left, right);
    }

    public static bool operator !=(Script? left, Script? right)
    {
        return !(left == right);
    }
}

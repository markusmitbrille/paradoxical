using Paradoxical.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model.Entities;

[Table("mods")]
public class Mod : IEntity, IModel, IEquatable<Mod?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("mod_name")]
    public string ModName { get => modName; set => modName = value; }
    public string modName = "New Mod";

    [Column("mod_version")]
    public string ModVersion { get => modVersion; set => modVersion = value; }
    public string modVersion = "1.0";

    [Column("game_version")]
    public string GameVersion { get => gameVersion; set => gameVersion = value; }
    public string gameVersion = "2.*";

    [Column("prefix")]
    public string Prefix { get => prefix; set => prefix = value; }
    public string prefix = "mod";

    public void Write(TextWriter writer, string dir, string file)
    {
        writer.Indent().WriteLine($"version=\"{ModVersion}\"");

        writer.Indent().WriteLine("tags = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine("Events");

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");

        writer.Indent().WriteLine($"name=\"{ModName}\"");
        writer.Indent().WriteLine($"supported_version=\"{GameVersion}\"");
        writer.Indent().WriteLine($"path=\"mod/{file}\"");
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Mod);
    }

    public bool Equals(Mod? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Mod? left, Mod? right)
    {
        return EqualityComparer<Mod>.Default.Equals(left, right);
    }

    public static bool operator !=(Mod? left, Mod? right)
    {
        return !(left == right);
    }
}

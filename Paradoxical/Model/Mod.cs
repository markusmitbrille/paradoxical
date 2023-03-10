using Paradoxical.Core;
using SQLite;
using System.IO;

namespace Paradoxical.Model;

[Table("mods")]
public class Mod
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("mod_name"), NotNull]
    public string ModName { get => modName; set => modName = value; }
    public string modName = "";

    [Column("mod_version"), NotNull]
    public string ModVersion { get => modVersion; set => modVersion = value; }
    public string modVersion = "";

    [Column("game_version"), NotNull]
    public string GameVersion { get => gameVersion; set => gameVersion = value; }
    public string gameVersion = "";

    [Column("prefix"), NotNull]
    public string Prefix { get => prefix; set => prefix = value; }
    public string prefix = "";

    public void Write(TextWriter writer, string dir, string file)
    {
        writer.Indent().WriteLine($"version=\"{ModVersion}\"");

        writer.Indent().WriteLine("tags = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine("Events");

        ParadoxText.IndentLevel++;
        writer.Indent().WriteLine("}");

        writer.Indent().WriteLine($"name=\"{ModName}\"");
        writer.Indent().WriteLine($"supported_version=\"{GameVersion}\"");
        writer.Indent().WriteLine($"path=\"mod/{file}\"");
    }
}

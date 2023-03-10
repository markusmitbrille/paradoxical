using Paradoxical.Core;
using Paradoxical.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model.Elements;

[Table("effects")]
public class Effect : IElement, IEquatable<Effect?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = "";

    [Column("code"), NotNull]
    public string Code { get => code; set => code = value; }
    public string code = "";

    [Column("tooltip"), NotNull]
    public string Tooltip { get => tooltip; set => tooltip = value; }
    public string tooltip = "";

    [Column("is_hidden"), NotNull]
    public bool Hidden { get => hidden; set => hidden = value; }
    public bool hidden = false;

    public void Write(
        TextWriter writer,
        IModService modService)
    {
        writer.Indent().WriteLine($"{modService.GetPrefix()}_{Name} = {{");
        ParadoxText.IndentLevel++;

        if (Hidden == true)
        {
            writer.Indent().WriteLine("hidden_effect = {");
            ParadoxText.IndentLevel++;
        }
        else if (Tooltip != string.Empty)
        {
            writer.Indent().WriteLine("custom_tooltip = {");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"text = {modService.GetPrefix()}.{Name}.tt");
            writer.WriteLine();
        }

        foreach (string line in Code.Split(Environment.NewLine))
        {
            writer.Indent().WriteLine(line);
        }

        if (Hidden == true)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
        else if (Tooltip != string.Empty)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService)
    {
        writer.WriteLocLine($"{modService.GetPrefix()}.{Name}.tt", Tooltip);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Effect);
    }

    public bool Equals(Effect? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Effect? left, Effect? right)
    {
        return EqualityComparer<Effect>.Default.Equals(left, right);
    }

    public static bool operator !=(Effect? left, Effect? right)
    {
        return !(left == right);
    }
}

using Paradoxical.Core;
using Paradoxical.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.Model.Elements;

[Table("triggers")]
public class Trigger : IElement, IEquatable<Trigger?>
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

    public Trigger()
    {
    }

    public Trigger(Trigger other)
    {
        id = 0;

        name = other.name;
        code = other.code;
        tooltip = other.tooltip;
    }

    public void Write(
        TextWriter writer,
        IModService modService)
    {
        writer.Indent().WriteLine($"{modService.GetPrefix()}_{Name} = {{");
        ParadoxText.IndentLevel++;

        if (Tooltip != string.Empty)
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

        if (Tooltip != string.Empty)
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
        return Equals(obj as Trigger);
    }

    public bool Equals(Trigger? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Trigger? left, Trigger? right)
    {
        return EqualityComparer<Trigger>.Default.Equals(left, right);
    }

    public static bool operator !=(Trigger? left, Trigger? right)
    {
        return !(left == right);
    }
}
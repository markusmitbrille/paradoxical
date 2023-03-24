using Paradoxical.Core;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Elements;

[Table("on_actions")]
public class OnAction : IEntity, IModel, IElement, IEquatable<OnAction?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = "";

    [Column("override_vanilla"), NotNull]
    public bool Vanilla { get => vanilla; set => vanilla = value; }
    public bool vanilla = false;

    [Column("random_event_chance"), NotNull]
    public int Chance { get => chance; set => chance = value; }
    public int chance = 100;

    public string GetQualifiedName(IModService modService)
    {
        return $"{modService.GetPrefix()}_act_{Id}";
    }

    public string GetLocationKey(IModService modService)
    {
        return $"{modService.GetPrefix()}.action.{Id}";
    }

    public void Write(
        TextWriter writer,
        IModService modService,
        IOnActionService onActionService)
    {
        if (Vanilla == false)
        {
            writer.Indent().WriteLine($"{GetQualifiedName(modService)} = {{");
            ParadoxText.IndentLevel++;
        }
        else
        {
            writer.Indent().WriteLine($"{Name} = {{");
            ParadoxText.IndentLevel++;
        }

        WriteTrigger(writer, modService, onActionService);

        writer.WriteLine();
        WriteEffect(writer, modService, onActionService);

        writer.WriteLine();
        WriteRandoms(writer, modService, onActionService);

        writer.WriteLine();
        WriteChildren(writer, modService, onActionService);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTrigger(
        TextWriter writer,
        IModService modService,
        IOnActionService onActionService)
    {
        var triggers = onActionService.GetTriggers(this);
        if (triggers.Any() == false)
        {
            writer.Indent().WriteLine("# no trigger");
            return;
        }

        writer.Indent().WriteLine("trigger = {");
        ParadoxText.IndentLevel++;

        foreach (Trigger trg in triggers)
        {
            writer.Indent().WriteLine($"{trg.GetQualifiedName(modService)} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteEffect(
        TextWriter writer,
        IModService modService,
        IOnActionService onActionService)
    {
        var effects = onActionService.GetEffects(this);
        if (effects.Any() == false)
        {
            writer.Indent().WriteLine("# no effect");
            return;
        }

        writer.Indent().WriteLine("effect = {");
        ParadoxText.IndentLevel++;

        foreach (Effect eff in effects)
        {
            writer.Indent().WriteLine($"{eff.GetQualifiedName(modService)} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteRandoms(
        TextWriter writer,
        IModService modService,
        IOnActionService onActionService)
    {
        var randoms = onActionService.GetRandoms(this);
        if (randoms.Any() == false)
        {
            writer.Indent().WriteLine("# no randoms");
            return;
        }

        writer.Indent().WriteLine("random_events = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"chance_to_happen = {Chance}");
        writer.WriteLine();

        foreach (Event evt in randoms)
        {
            writer.Indent().WriteLine($"{evt.Weight} = {evt.GetQualifiedName(modService)}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteChildren(
        TextWriter writer,
        IModService modService,
        IOnActionService onActionService)
    {
        var children = onActionService.GetChildren(this);
        if (children.Any() == false)
        {
            writer.Indent().WriteLine("# no on-actions");
            return;
        }

        writer.Indent().WriteLine("on_actions = {");
        ParadoxText.IndentLevel++;

        foreach (OnAction act in children)
        {
            if (act.Vanilla == false)
            {
                writer.Indent().WriteLine($"{act.GetQualifiedName(modService)}");
            }
            else
            {
                writer.Indent().WriteLine($"{act.Name}");
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as OnAction);
    }

    public bool Equals(OnAction? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(OnAction? left, OnAction? right)
    {
        return EqualityComparer<OnAction>.Default.Equals(left, right);
    }

    public static bool operator !=(OnAction? left, OnAction? right)
    {
        return !(left == right);
    }
}

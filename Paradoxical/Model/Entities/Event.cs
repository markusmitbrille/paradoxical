using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Elements;

[Table("events")]
public class Event : IEntity, IModel, IElement, IEquatable<Event?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = $"evt_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("title"), NotNull]
    public string Title { get => title; set => title = value; }
    public string title = "";

    [Column("description"), NotNull]
    public string Description { get => description; set => description = value; }
    public string description = "";

    [Column("theme"), NotNull]
    public string Theme { get => theme; set => theme = value; }
    public string theme = "";

    [Column("hidden"), NotNull]
    public bool Hidden { get => hidden; set => hidden = value; }
    public bool hidden;

    [Column("cooldown"), NotNull]
    public int Cooldown { get => cooldown; set => cooldown = value; }
    public int cooldown;

    [Column("custom_trigger"), NotNull]
    public string CustomTrigger { get => customTrigger; set => customTrigger = value; }
    public string customTrigger = "";

    [Column("custom_immediate"), NotNull]
    public string CustomImmediateEffect { get => customImmediateEffect; set => customImmediateEffect = value; }
    public string customImmediateEffect = "";

    [Column("custom_after"), NotNull]
    public string CustomAfterEffect { get => customAfterEffect; set => customAfterEffect = value; }
    public string customAfterEffect = "";

    public Event()
    {
    }

    public Event(Event other)
    {
        id = 0;

        name = other.name;
        title = other.title;
        description = other.description;
        theme = other.theme;
        hidden = other.hidden;
        cooldown = other.cooldown;

        customTrigger = other.customTrigger;
        customImmediateEffect = other.customImmediateEffect;
        customAfterEffect = other.customAfterEffect;
    }

    public string GetQualifiedName(IModService modService)
    {
        return $"{modService.GetPrefix()}_evt_{Id}";
    }

    public string GetLocationKey(IModService modService)
    {
        return $"{modService.GetPrefix()}.event.{Id}";
    }

    private const string DEFAULT_THEME = "default";

    public void Write(
        TextWriter writer,
        IModService modService,
        IEventService eventService,
        IOptionService optionService,
        IPortraitService portraitService)
    {
        writer.Indent().WriteLine($"{GetQualifiedName(modService)} = {{");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"type = character_event");

        if (Hidden)
        {
            writer.Indent().WriteLine("hidden = yes");
        }

        if (Hidden == false)
        {
            writer.WriteLine();

            writer.Indent().WriteLine($"title = {GetLocationKey(modService)}.t");
            writer.Indent().WriteLine($"desc = {GetLocationKey(modService)}.d");
            writer.Indent().WriteLine($"theme = {(Theme == string.Empty ? DEFAULT_THEME : Theme)}");
        }

        writer.WriteLine();
        WriteCooldown(writer);

        if (Hidden == false)
        {
            WritePortraits(writer, eventService, portraitService);
        }

        writer.WriteLine();
        WriteTrigger(writer, modService, eventService);

        writer.WriteLine();
        WriteImmediate(writer, modService, eventService);

        writer.WriteLine();
        WriteAfter(writer, modService, eventService);

        if (Hidden == false)
        {
            writer.WriteLine();

            WriteOptions(writer, modService, eventService, optionService);
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteCooldown(
        TextWriter writer)
    {
        if (Cooldown <= 0)
        {
            writer.Indent().WriteLine("# no cooldown");
            return;
        }

        writer.Indent().WriteLine($"cooldown = {{ days = {Cooldown} }}");
    }

    private void WritePortraits(
        TextWriter writer,
        IEventService eventService,
        IPortraitService portraitService)
    {
        writer.WriteLine();

        var leftPortrait = eventService.GetLeftPortrait(this);
        var rightPortrait = eventService.GetRightPortrait(this);
        var lowerLeftPortrait = eventService.GetLowerLeftPortrait(this);
        var lowerCenterPortrait = eventService.GetLowerCenterPortrait(this);
        var lowerRightPortrait = eventService.GetLowerRightPortrait(this);

        leftPortrait.Write(writer, portraitService);
        rightPortrait.Write(writer, portraitService);
        lowerLeftPortrait.Write(writer, portraitService);
        lowerCenterPortrait.Write(writer, portraitService);
        lowerRightPortrait.Write(writer, portraitService);
    }

    private void WriteTrigger(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var triggers = eventService.GetTriggers(this);
        if (triggers.Any() == false && CustomTrigger.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no trigger");
            return;
        }

        writer.Indent().WriteLine("trigger = {");
        ParadoxText.IndentLevel++;

        if (triggers.Any() == true)
        {
            writer.Indent().WriteLine("# scripted triggers");

            foreach (Trigger trg in triggers)
            {
                writer.Indent().WriteLine($"{trg.GetQualifiedName(modService)} = yes");
            }
        }

        if (CustomTrigger.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom trigger");

            foreach (string line in CustomTrigger.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteImmediate(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var effects = eventService.GetImmediates(this);
        if (effects.Any() == false && CustomImmediateEffect.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no immediate");
            return;
        }

        writer.Indent().WriteLine("immediate = {");
        ParadoxText.IndentLevel++;

        if (effects.Any() == true)
        {
            writer.Indent().WriteLine("# scripted effects");

            foreach (Effect eff in effects)
            {
                writer.Indent().WriteLine($"{eff.GetQualifiedName(modService)} = yes");
            }
        }

        if (CustomImmediateEffect.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom effect");

            foreach (string line in CustomImmediateEffect.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteAfter(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var effects = eventService.GetAfters(this);
        if (effects.Any() == false && CustomAfterEffect.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no after");
            return;
        }

        writer.Indent().WriteLine("after = {");
        ParadoxText.IndentLevel++;

        if (effects.Any() == true)
        {
            writer.Indent().WriteLine("# scripted effects");

            foreach (Effect eff in effects)
            {
                writer.Indent().WriteLine($"{eff.GetQualifiedName(modService)} = yes");
            }
        }

        if (CustomAfterEffect.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom effect");

            foreach (string line in CustomAfterEffect.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteOptions(
        TextWriter writer,
        IModService modService,
        IEventService eventService,
        IOptionService optionService)
    {
        var options = eventService.GetOptions(this).ToList();
        if (options.Any() == false)
        {
            writer.Indent().WriteLine("# no options");
            return;
        }

        options.Sort();
        foreach (Option opt in options)
        {
            opt.Write(writer, modService, optionService);
        }
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService)
    {
        writer.WriteLocLine($"{GetLocationKey(modService)}.t", Title);
        writer.WriteLocLine($"{GetLocationKey(modService)}.d", Description);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Event);
    }

    public bool Equals(Event? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Event? left, Event? right)
    {
        return EqualityComparer<Event>.Default.Equals(left, right);
    }

    public static bool operator !=(Event? left, Event? right)
    {
        return !(left == right);
    }
}

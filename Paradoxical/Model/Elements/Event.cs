using Paradoxical.Core;
using Paradoxical.Model.Components;
using Paradoxical.Services;
using Paradoxical.Services.Components;
using Paradoxical.Services.Elements;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Elements;

[Table("events")]
public class Event : IElement, IEquatable<Event?>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = "";

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

    [Column("weight"), NotNull]
    public int Weight { get => weight; set => weight = value; }
    public int weight;

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
        weight = other.weight;
        cooldown = other.cooldown;
        customTrigger = other.customTrigger;
        customImmediateEffect = other.customImmediateEffect;
        customAfterEffect = other.customAfterEffect;
    }

    private const string DEFAULT_THEME = "default";

    public void Write(
        TextWriter writer,
        IModService modService,
        IEventService eventService,
        IOptionService optionService,
        IPortraitService portraitService)
    {
        writer.Indent().WriteLine($"{modService.GetPrefix()}.{Id} = {{");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"type = character_event");

        if (Hidden)
        {
            writer.Indent().WriteLine("hidden = yes");
        }

        if (Hidden == false)
        {
            writer.WriteLine();

            writer.Indent().WriteLine($"title = {modService.GetPrefix()}.{Id}.t");
            writer.Indent().WriteLine($"desc = {modService.GetPrefix()}.{Id}.d");
            writer.Indent().WriteLine($"theme = {(Theme == string.Empty ? DEFAULT_THEME : Theme)}");
        }

        writer.WriteLine();
        WriteCooldown(writer);

        if (Hidden == false)
        {
            writer.WriteLine();

            WriteLeftPortrait(writer, portraitService);
            WriteRightPortrait(writer, portraitService);
            WriteLowerLeftPortrait(writer, portraitService);
            WriteLowerCenterPortrait(writer, portraitService);
            WriteLowerRightPortrait(writer, portraitService);
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

    private void WriteLeftPortrait(
        TextWriter writer,
        IPortraitService portraitService)
    {
        var portrait = portraitService.Get(this, PortraitPosition.Left);
        if (portrait == null)
        {
            writer.Indent().WriteLine("# no left portrait");
            return;
        }

        writer.Indent().WriteLine("left_portrait = {");
        ParadoxText.IndentLevel++;

        portrait.Write(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteRightPortrait(
        TextWriter writer,
        IPortraitService portraitService)
    {
        var portrait = portraitService.Get(this, PortraitPosition.Right);
        if (portrait == null)
        {
            writer.Indent().WriteLine("# no right portrait");
            return;
        }

        writer.Indent().WriteLine("right_portrait = {");
        ParadoxText.IndentLevel++;

        portrait.Write(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteLowerLeftPortrait(
        TextWriter writer,
        IPortraitService portraitService)
    {
        var portrait = portraitService.Get(this, PortraitPosition.LowerLeft);
        if (portrait == null)
        {
            writer.Indent().WriteLine("# no lower left portrait");
            return;
        }

        writer.Indent().WriteLine("lower_left_portrait = {");
        ParadoxText.IndentLevel++;

        portrait.Write(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteLowerCenterPortrait(
        TextWriter writer,
        IPortraitService portraitService)
    {
        var portrait = portraitService.Get(this, PortraitPosition.LowerCenter);
        if (portrait == null)
        {
            writer.Indent().WriteLine("# no lower center portrait");
            return;
        }

        writer.Indent().WriteLine("lower_center_portrait = {");
        ParadoxText.IndentLevel++;

        portrait.Write(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteLowerRightPortrait(
        TextWriter writer,
        IPortraitService portraitService)
    {
        var portrait = portraitService.Get(this, PortraitPosition.LowerRight);
        if (portrait == null)
        {
            writer.Indent().WriteLine("# no lower right portrait");
            return;
        }

        writer.Indent().WriteLine("lower_right_portrait = {");
        ParadoxText.IndentLevel++;

        portrait.Write(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTrigger(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var triggers = eventService.GetTriggers(this);
        if (triggers.Any() == false)
        {
            writer.Indent().WriteLine("# no trigger");
            return;
        }

        writer.Indent().WriteLine("trigger = {");
        ParadoxText.IndentLevel++;

        foreach (Trigger trg in triggers)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{trg.Name} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteImmediate(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var effects = eventService.GetImmediateEffects(this);
        if (effects.Any() == false)
        {
            writer.Indent().WriteLine("# no immediate");
            return;
        }

        writer.Indent().WriteLine("immediate = {");
        ParadoxText.IndentLevel++;

        foreach (Effect eff in effects)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{eff.Name} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteAfter(
        TextWriter writer,
        IModService modService,
        IEventService eventService)
    {
        var effects = eventService.GetImmediateEffects(this);
        if (effects.Any() == false)
        {
            writer.Indent().WriteLine("# no after");
            return;
        }

        writer.Indent().WriteLine("after = {");
        ParadoxText.IndentLevel++;

        foreach (Effect eff in effects)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{eff.Name} = yes");
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
        var options = eventService.GetOptions(this);
        if (options.Any() == false)
        {
            writer.Indent().WriteLine("# no options");
            return;
        }

        foreach (Option opt in options)
        {
            opt.Write(writer, modService, optionService);
        }
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService,
        IEventService eventService,
        IOptionService optionService)
    {
        writer.WriteLocLine($"{modService.GetPrefix()}.{Id}.t", Title);
        writer.WriteLocLine($"{modService.GetPrefix()}.{Id}.d", Description);

        var options = eventService.GetOptions(this);
        foreach (Option opt in options)
        {
            opt.WriteLoc(writer, modService, optionService);
        }
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

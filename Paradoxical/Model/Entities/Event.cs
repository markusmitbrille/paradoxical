﻿using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Entities;

[Table("events")]
public class Event : IEntity, IModel, IEquatable<Event?>, IComparable<Event>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed]
    public string Name { get => name; set => name = value; }
    public string name = $"evt_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("title")]
    public string Title { get => title; set => title = value; }
    public string title = "";

    [Column("description")]
    public string Description { get => description; set => description = value; }
    public string description = "";

    [Column("theme")]
    public string Theme { get => theme; set => theme = value; }
    public string theme = "";

    [Column("background")]
    public string Background { get => background; set => background = value; }
    public string background = "";

    [Column("type")]
    public string Type { get => type; set => type = value; }
    public string type = "character_event";

    [Column("scope")]
    public string Scope { get => scope; set => scope = value; }
    public string scope = "none";

    [Column("hidden")]
    public bool Hidden { get => hidden; set => hidden = value; }
    public bool hidden;

    [Column("cooldown")]
    public int Cooldown { get => cooldown; set => cooldown = value; }
    public int cooldown;

    [Column("custom_trigger")]
    public string CustomTrigger { get => customTrigger; set => customTrigger = value; }
    public string customTrigger = "";

    [Column("custom_immediate_effect")]
    public string CustomImmediateEffect { get => customImmediateEffect; set => customImmediateEffect = value; }
    public string customImmediateEffect = "";

    [Column("custom_after_effect")]
    public string CustomAfterEffect { get => customAfterEffect; set => customAfterEffect = value; }
    public string customAfterEffect = "";

    public string GetQualifiedName(IModService modService)
    {
        return $"{modService.GetPrefix()}.{Id}";
    }

    public string GetLocationKey(IModService modService)
    {
        return $"{modService.GetPrefix()}.event.{Id}";
    }

    public void Write(
        TextWriter writer,
        IModService modService,
        IEventService eventService,
        IOptionService optionService,
        IPortraitService portraitService,
        ILinkService linkService)
    {
        writer.Indent().WriteLine($"{GetQualifiedName(modService)} = {{");
        ParadoxText.IndentLevel++;

        if (Hidden == true)
        {
            writer.Indent().WriteLine("hidden = yes");
            writer.Indent().WriteLine($"scope = {Scope}");
        }

        if (Hidden == false)
        {
            writer.Indent().WriteLine($"type = {Type}");

            writer.WriteLine();

            writer.Indent().WriteLine($"title = {GetLocationKey(modService)}.t");
            writer.Indent().WriteLine($"desc = {GetLocationKey(modService)}.d");

            if (Theme.IsEmpty() == false)
            {
                writer.Indent().WriteLine($"theme = {Theme}");
            }
            if (Background.IsEmpty() == false)
            {
                writer.Indent().WriteLine($"override_background = {{ reference = {Background} }}");
            }
        }

        writer.WriteLine();
        WriteCooldown(writer);

        if (Hidden == false)
        {
            writer.WriteLine();
            WritePortraits(writer, eventService, portraitService);
        }

        writer.WriteLine();
        WriteTrigger(writer);

        writer.WriteLine();
        WriteImmediate(writer, modService, eventService, linkService);

        writer.WriteLine();
        WriteAfter(writer);

        if (Hidden == false)
        {
            writer.WriteLine();
            WriteOptions(writer, modService, eventService, optionService, linkService);
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
        var leftPortrait = eventService.GetLeftPortrait(this);
        var rightPortrait = eventService.GetRightPortrait(this);
        var lowerLeftPortrait = eventService.GetLowerLeftPortrait(this);
        var lowerCenterPortrait = eventService.GetLowerCenterPortrait(this);
        var lowerRightPortrait = eventService.GetLowerRightPortrait(this);

        leftPortrait?.Write(writer, portraitService);
        rightPortrait?.Write(writer, portraitService);
        lowerLeftPortrait?.Write(writer, portraitService);
        lowerCenterPortrait?.Write(writer, portraitService);
        lowerRightPortrait?.Write(writer, portraitService);
    }

    private void WriteTrigger(
        TextWriter writer)
    {
        if (CustomTrigger.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no trigger");
            return;
        }

        writer.Indent().WriteLine("trigger = {");
        ParadoxText.IndentLevel++;

        if (CustomTrigger.IsEmpty() == false)
        {
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
        IEventService eventService,
        ILinkService linkService)
    {
        var links = eventService.GetLinks(this);

        if (CustomImmediateEffect.IsEmpty() == true && links.Any() == false)
        {
            writer.Indent().WriteLine("# no immediate");
            return;
        }

        writer.Indent().WriteLine("immediate = {");
        ParadoxText.IndentLevel++;

        if (CustomImmediateEffect.IsEmpty() == false)
        {
            foreach (string line in CustomImmediateEffect.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }
        else
        {
            writer.Indent().WriteLine("# no custom effect");
        }

        if (links.Any() == true)
        {
            writer.WriteLine();
            writer.Indent().WriteLine("# follow-up events");

            foreach (var link in links)
            {
                link.Write(writer, modService, linkService);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteAfter(
        TextWriter writer)
    {
        if (CustomAfterEffect.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no after");
            return;
        }

        writer.Indent().WriteLine("after = {");
        ParadoxText.IndentLevel++;

        if (CustomAfterEffect.IsEmpty() == false)
        {
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
        IOptionService optionService,
        ILinkService linkService)
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
            opt.Write(writer, modService, optionService, linkService);
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

    public int CompareTo(Event? other)
    {
        return Comparer<string?>.Default.Compare(name, other?.name);
    }

    public static bool operator ==(Event? left, Event? right)
    {
        return EqualityComparer<Event>.Default.Equals(left, right);
    }

    public static bool operator !=(Event? left, Event? right)
    {
        return !(left == right);
    }

    public static bool operator <(Event left, Event right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Event left, Event right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Event left, Event right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Event left, Event right)
    {
        return left.CompareTo(right) >= 0;
    }
}

﻿using Paradoxical.Core;
using Paradoxical.Extensions;
using Paradoxical.Services.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model.Entities;

[Table("options")]
public class Option : IEntity, IModel, IEquatable<Option?>, IComparable<Option>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

    [Column("name"), Indexed]
    public string Name { get => name; set => name = value; }
    public string name = $"opt_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("title")]
    public string Title { get => title; set => title = value; }
    public string title = "";

    [Column("tooltip")]
    public string Tooltip { get => tooltip; set => tooltip = value; }
    public string tooltip = "";

    [Column("priority")]
    public int? Priority { get => priority; set => priority = value; }
    public int? priority = null;

    [Column("custom_trigger")]
    public string CustomTrigger { get => customTrigger; set => customTrigger = value; }
    public string customTrigger = "";

    [Column("custom_effect")]
    public string CustomEffect { get => customEffect; set => customEffect = value; }
    public string customEffect = "";

    [Column("ai_base_chance")]
    public int AiBaseChance { get => aiBaseChance; set => aiBaseChance = value; }
    public int aiBaseChance;

    [Column("ai_custom_chance")]
    public string AiCustomChance { get => aiCustomChance; set => aiCustomChance = value; }
    public string aiCustomChance = "";

    [Column("ai_boldness")]
    public int AiBoldnessTargetModifier { get => aiBoldnessTargetModifier; set => aiBoldnessTargetModifier = value; }
    public int aiBoldnessTargetModifier;

    [Column("ai_compassion")]
    public int AiCompassionTargetModifier { get => aiCompassionTargetModifier; set => aiCompassionTargetModifier = value; }
    public int aiCompassionTargetModifier;

    [Column("ai_greed")]
    public int AiGreedTargetModifier { get => aiGreedTargetModifier; set => aiGreedTargetModifier = value; }
    public int aiGreedTargetModifier;

    [Column("ai_energy")]
    public int AiEnergyTargetModifier { get => aiEnergyTargetModifier; set => aiEnergyTargetModifier = value; }
    public int aiEnergyTargetModifier;

    [Column("ai_honor")]
    public int AiHonorTargetModifier { get => aiHonorTargetModifier; set => aiHonorTargetModifier = value; }
    public int aiHonorTargetModifier;

    [Column("ai_rationality")]
    public int AiRationalityTargetModifier { get => aiRationalityTargetModifier; set => aiRationalityTargetModifier = value; }
    public int aiRationalityTargetModifier;

    [Column("ai_sociability")]
    public int AiSociabilityTargetModifier { get => aiSociabilityTargetModifier; set => aiSociabilityTargetModifier = value; }
    public int aiSociabilityTargetModifier;

    [Column("ai_vengefulness")]
    public int AiVengefulnessTargetModifier { get => aiVengefulnessTargetModifier; set => aiVengefulnessTargetModifier = value; }
    public int aiVengefulnessTargetModifier;

    [Column("ai_zeal")]
    public int AiZealTargetModifier { get => aiZealTargetModifier; set => aiZealTargetModifier = value; }
    public int aiZealTargetModifier;

    public string GetQualifiedName(IModService modService)
    {
        return $"{modService.GetPrefix()}_opt_{Id}";
    }

    public string GetLocationKey(IModService modService)
    {
        return $"{modService.GetPrefix()}.option.{Id}";
    }

    public void Write(
        TextWriter writer,
        IModService modService,
        IOptionService optionService,
        ILinkService linkService)
    {
        writer.Indent().WriteLine("option = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"name = {GetLocationKey(modService)}.t");

        WriteTooltip(writer, modService);

        writer.WriteLine();
        WriteTrigger(writer);

        writer.WriteLine();
        WriteAiChance(writer);

        writer.WriteLine();
        WriteEffect(writer);

        writer.WriteLine();
        WriteLinks(writer, modService, optionService, linkService);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTooltip(
        TextWriter writer,
        IModService modService)
    {
        if (Tooltip == string.Empty)
        {
            writer.Indent().WriteLine("# no custom tooltip");
            return;
        }

        writer.Indent().WriteLine($"custom_tooltip = {GetLocationKey(modService)}.tt");
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

    private void WriteAiChance(
        TextWriter writer)
    {
        writer.Indent().WriteLine("ai_chance = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"base = {AiBaseChance}");

        if (AiBoldnessTargetModifier == 0
            && AiCompassionTargetModifier == 0
            && AiGreedTargetModifier == 0
            && AiEnergyTargetModifier == 0
            && AiHonorTargetModifier == 0
            && AiRationalityTargetModifier == 0
            && AiSociabilityTargetModifier == 0
            && AiVengefulnessTargetModifier == 0
            && AiZealTargetModifier == 0)
        {
            writer.WriteLine();
            writer.Indent().WriteLine("# no ai target modifiers");
        }
        else
        {
            writer.WriteLine();

            if (AiBoldnessTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_boldness_target_modifier = {{ VALUE = {AiBoldnessTargetModifier} }}");
            }
            if (AiCompassionTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_compassion_target_modifier = {{ VALUE = {AiCompassionTargetModifier} }}");
            }
            if (AiGreedTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_greed_target_modifier = {{ VALUE = {AiGreedTargetModifier} }}");
            }
            if (AiEnergyTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_energy_target_modifier = {{ VALUE = {AiEnergyTargetModifier} }}");
            }
            if (AiHonorTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_honor_target_modifier = {{ VALUE = {AiHonorTargetModifier} }}");
            }
            if (AiRationalityTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_rationality_target_modifier = {{ VALUE = {AiRationalityTargetModifier} }}");
            }
            if (AiSociabilityTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_sociability_target_modifier = {{ VALUE = {AiSociabilityTargetModifier} }}");
            }
            if (AiVengefulnessTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_vengefulness_target_modifier = {{ VALUE = {AiVengefulnessTargetModifier} }}");
            }
            if (AiZealTargetModifier != 0)
            {
                writer.Indent().WriteLine($"ai_zeal_target_modifier = {{ VALUE = {AiZealTargetModifier} }}");
            }
        }

        if (AiCustomChance.IsEmpty() == false)
        {
            writer.WriteLine();
            writer.Indent().WriteLine("# custom ai chance");

            foreach (string line in AiCustomChance.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteLinks(
        TextWriter writer,
        IModService modService,
        IOptionService optionService,
        ILinkService linkService)
    {
        var links = optionService.GetLinks(this);
        if (links.Any() == false)
        {
            writer.Indent().WriteLine("# no follow-up events");
            return;
        }

        foreach (var link in links)
        {
            link.Write(writer, modService, linkService);
        }
    }

    private void WriteEffect(
        TextWriter writer)
    {
        if (CustomEffect.IsEmpty() != false)
        {
            writer.Indent().WriteLine("# no custom effect");
            return;
        }

        foreach (string line in CustomEffect.Split(ParadoxText.NewParagraph))
        {
            writer.Indent().WriteLine(line);
        }
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService)
    {
        writer.WriteLocLine($"{GetLocationKey(modService)}.t", Title);
        writer.WriteLocLine($"{GetLocationKey(modService)}.tt", Tooltip);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Option);
    }

    public bool Equals(Option? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public int CompareTo(Option? other)
    {
        return Comparer<int?>.Default.Compare(priority, other?.priority);
    }

    public static bool operator ==(Option? left, Option? right)
    {
        return EqualityComparer<Option>.Default.Equals(left, right);
    }

    public static bool operator !=(Option? left, Option? right)
    {
        return !(left == right);
    }

    public static bool operator <(Option left, Option right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Option left, Option right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Option left, Option right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Option left, Option right)
    {
        return left.CompareTo(right) >= 0;
    }
}

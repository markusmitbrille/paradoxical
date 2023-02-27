using Paradoxical.Core;
using Paradoxical.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model;

[Table("options")]
public class Option : IEquatable<Option?>
{
    [Column("ID"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("event_id"), Indexed, NotNull]
    public int EventId { get => eventId; set => eventId = value; }
    public int eventId;

    [Column("title"), NotNull]
    public string Title { get => title; set => title = value; }
    public string title = "";

    [Column("tooltip"), NotNull]
    public string Tooltip { get => tooltip; set => tooltip = value; }
    public string tooltip = "";

    [Column("triggered_event_id"), Indexed]
    public int? TriggeredEventId { get => triggeredEventId; set => triggeredEventId = value; }
    public int? triggeredEventId;

    [Column("triggered_event_scope"), NotNull]
    public string TriggeredEventScope { get => triggeredEventScope; set => triggeredEventScope = value; }
    public string triggeredEventScope = "";

    [Column("triggered_event_min_days")]
    public int TriggeredEventMinDays { get => triggeredEventMinDays; set => triggeredEventMinDays = value; }
    public int triggeredEventMinDays;

    [Column("triggered_event_max_days")]
    public int TriggeredEventMaxDays { get => triggeredEventMaxDays; set => triggeredEventMaxDays = value; }
    public int triggeredEventMaxDays;

    [Column("ai_base_chance"), NotNull]
    public int AiBaseChance { get => aiBaseChance; set => aiBaseChance = value; }
    public int aiBaseChance;

    [Column("ai_boldness"), NotNull]
    public int AiBoldnessTargetModifier { get => aiBoldnessTargetModifier; set => aiBoldnessTargetModifier = value; }
    public int aiBoldnessTargetModifier;

    [Column("ai_compassion"), NotNull]
    public int AiCompassionTargetModifier { get => aiCompassionTargetModifier; set => aiCompassionTargetModifier = value; }
    public int aiCompassionTargetModifier;

    [Column("ai_greed"), NotNull]
    public int AiGreedTargetModifier { get => aiGreedTargetModifier; set => aiGreedTargetModifier = value; }
    public int aiGreedTargetModifier;

    [Column("ai_energy"), NotNull]
    public int AiEnergyTargetModifier { get => aiEnergyTargetModifier; set => aiEnergyTargetModifier = value; }
    public int aiEnergyTargetModifier;

    [Column("ai_honor"), NotNull]
    public int AiHonorTargetModifier { get => aiHonorTargetModifier; set => aiHonorTargetModifier = value; }
    public int aiHonorTargetModifier;

    [Column("ai_rationality"), NotNull]
    public int AiRationalityTargetModifier { get => aiRationalityTargetModifier; set => aiRationalityTargetModifier = value; }
    public int aiRationalityTargetModifier;

    [Column("ai_sociability"), NotNull]
    public int AiSociabilityTargetModifier { get => aiSociabilityTargetModifier; set => aiSociabilityTargetModifier = value; }
    public int aiSociabilityTargetModifier;

    [Column("ai_vengefulness"), NotNull]
    public int AiVengefulnessTargetModifier { get => aiVengefulnessTargetModifier; set => aiVengefulnessTargetModifier = value; }
    public int aiVengefulnessTargetModifier;

    [Column("ai_zeal"), NotNull]
    public int AiZealTargetModifier { get => aiZealTargetModifier; set => aiZealTargetModifier = value; }
    public int aiZealTargetModifier;

    public void Write(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        Event owner = optionService.GetOwner(this);

        writer.Indent().WriteLine("option = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"name = {modService.GetPrefix()}.{owner.Id}.o.{Id}");

        WriteTooltip(writer, modService, optionService);

        writer.WriteLine();
        WriteTrigger(writer, modService, optionService);

        writer.WriteLine();
        WriteAiChance(writer);

        writer.WriteLine();
        WriteTriggeredEvent(writer, modService, optionService);

        writer.WriteLine();
        WriteEffect(writer, modService, optionService);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTooltip(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        if (Tooltip == string.Empty)
        {
            writer.Indent().WriteLine("# no custom tooltip");
            return;
        }

        Event owner = optionService.GetOwner(this);

        writer.Indent().WriteLine($"custom_tooltip = {modService.GetPrefix()}.{owner.Id}.o.{Id}.tt");
    }

    private void WriteTrigger(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        var triggers = optionService.GetTriggers(this);
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
            writer.Indent().WriteLine("# no ai target modifiers");
        }

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

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTriggeredEvent(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        var triggeredEvent = optionService.GetTriggeredEvent(this);
        if (triggeredEvent == null)
        {
            writer.Indent().WriteLine("# no follow-up event");
            return;
        }

        writer.Indent().WriteLine("# follow-up event");

        if (TriggeredEventScope != string.Empty)
        {
            writer.Indent().WriteLine($"{TriggeredEventScope} = {{");
            ParadoxText.IndentLevel++;
        }

        writer.Indent().WriteLine("trigger_event = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"id = {modService.GetPrefix()}.{triggeredEvent.Id}");
        writer.Indent().WriteLine($"days = {{ {TriggeredEventMinDays} {TriggeredEventMaxDays} }}");

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");

        if (TriggeredEventScope != string.Empty)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }

    private void WriteEffect(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        var effects = optionService.GetEffects(this);
        if (effects.Any() == false)
        {
            writer.Indent().WriteLine("# no special effect");
            return;
        }

        foreach (Effect eff in effects)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{eff.Name} = yes");
        }
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService,
        IOptionService optionService)
    {
        Event owner = optionService.GetOwner(this);

        writer.WriteLocLine($"{modService.GetPrefix()}.{owner.Id}.o.{Id}", Title);
        writer.WriteLocLine($"{modService.GetPrefix()}.{owner.Id}.o.{Id}.tt", Tooltip);
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

    public static bool operator ==(Option? left, Option? right)
    {
        return EqualityComparer<Option>.Default.Equals(left, right);
    }

    public static bool operator !=(Option? left, Option? right)
    {
        return !(left == right);
    }
}
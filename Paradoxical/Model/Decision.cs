using Paradoxical.Core;
using Paradoxical.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Paradoxical.Model;

[Table("decisions")]
public class Decision : IElement, IEquatable<Decision?>
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

    [Column("tooltip"), NotNull]
    public string Tooltip { get => tooltip; set => tooltip = value; }
    public string tooltip = "";

    [Column("confirm"), NotNull]
    public string Confirm { get => confirm; set => confirm = value; }
    public string confirm = "";

    [Column("picture"), NotNull]
    public string Picture { get => picture; set => picture = value; }
    public string picture = "";

    [Column("is_major"), NotNull]
    public bool Major { get => major; set => major = value; }
    public bool major;

    [Column("sort_order"), NotNull]
    public int SortOrder { get => sortOrder; set => sortOrder = value; }
    public int sortOrder;

    [Column("cooldown"), NotNull]
    public int Cooldown { get => cooldown; set => cooldown = value; }
    public int cooldown;

    [Column("gold_cost"), NotNull]
    public int GoldCost { get => goldCost; set => goldCost = value; }
    public int goldCost;

    [Column("piety_cost"), NotNull]
    public int PietyCost { get => pietyCost; set => pietyCost = value; }
    public int pietyCost;

    [Column("prestige_cost"), NotNull]
    public int PrestigeCost { get => prestigeCost; set => prestigeCost = value; }
    public int prestigeCost;

    [Column("triggered_event_id"), Indexed]
    public int? TriggeredEventId { get => triggeredEventId; set => triggeredEventId = value; }
    public int? triggeredEventId;

    [Column("triggered_event_scope"), NotNull]
    public string TriggeredEventScope { get => triggeredEventScope; set => triggeredEventScope = value; }
    public string triggeredEventScope = "";

    [Column("ai_potential"), NotNull]
    public bool AiPotential { get => aiPotential; set => aiPotential = value; }
    public bool aiPotential;

    [Column("ai_goal"), NotNull]
    public bool AiGoal { get => aiGoal; set => aiGoal = value; }
    public bool aiGoal;

    [Column("ai_check_frequency"), NotNull]
    public int AiCheckFrequency { get => aiCheckFrequency; set => aiCheckFrequency = value; }
    public int aiCheckFrequency;

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

    private const string DEFAULT_PICTURE = "gfx/interface/illustrations/decisions/decision_misc.dds";

    public void Write(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        writer.Indent().WriteLine($"{modService.GetPrefix()}_{Name} = {{");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"title = {modService.GetPrefix()}.{Name}.t");
        writer.Indent().WriteLine($"desc = {modService.GetPrefix()}.{Name}.d");
        writer.Indent().WriteLine($"selection_tooltip = {modService.GetPrefix()}.{Name}.tt");
        writer.Indent().WriteLine($"confirm_text = {modService.GetPrefix()}.{Name}.c");
        writer.Indent().WriteLine($"picture = \"{(Picture == string.Empty ? DEFAULT_PICTURE : Picture)}\"");

        writer.WriteLine();

        writer.Indent().WriteLine($"major = {(Major ? "yes" : "no")}");
        writer.Indent().WriteLine($"sort_order = {SortOrder}");

        writer.WriteLine();
        WriteCooldown(writer);

        writer.WriteLine();
        WriteCost(writer);

        writer.WriteLine();
        WriteIsShownTrigger(writer, modService, decisionService);

        writer.WriteLine();
        WriteIsValidTrigger(writer, modService, decisionService);

        writer.WriteLine();
        WriteIsValidFailureTrigger(writer, modService, decisionService);

        writer.WriteLine();
        WriteEffect(writer, modService, decisionService);

        writer.WriteLine();
        WriteAiChance(writer);

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

    private void WriteCost(
        TextWriter writer)
    {
        if (GoldCost <= 0 && PietyCost <= 0 && PrestigeCost <= 0)
        {
            writer.Indent().WriteLine("# no cost");
            return;
        }

        writer.Indent().WriteLine("cost = {");
        ParadoxText.IndentLevel++;

        if (GoldCost > 0)
        {
            writer.Indent().WriteLine($"gold = {GoldCost}");
        }
        if (PietyCost > 0)
        {
            writer.Indent().WriteLine($"piety = {PietyCost}");
        }
        if (PrestigeCost > 0)
        {
            writer.Indent().WriteLine($"prestige = {PrestigeCost}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteIsShownTrigger(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var triggers = decisionService.GetIsShownTriggers(this);
        if (triggers.Any() == false)
        {
            writer.Indent().WriteLine("# no is-shown");
            return;
        }

        writer.Indent().WriteLine("is_shown = {");
        ParadoxText.IndentLevel++;

        foreach (Trigger trg in triggers)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{trg.Name} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteIsValidTrigger(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var triggers = decisionService.GetIsValidTriggers(this);
        if (triggers.Any() == false)
        {
            writer.Indent().WriteLine("# no is-valid");
            return;
        }

        writer.Indent().WriteLine("is_valid = {");
        ParadoxText.IndentLevel++;

        foreach (Trigger trg in triggers)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{trg.Name} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteIsValidFailureTrigger(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var triggers = decisionService.GetIsValidFailureTriggers(this);
        if (triggers.Any() == false)
        {
            writer.Indent().WriteLine("# no is-valid failure");
            return;
        }

        writer.Indent().WriteLine("is_valid_showing_failures_only = {");
        ParadoxText.IndentLevel++;

        foreach (Trigger trg in triggers)
        {
            writer.Indent().WriteLine($"{modService.GetPrefix()}_{trg.Name} = yes");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteEffect(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var effects = decisionService.GetEffects(this);
        var triggeredEvent = decisionService.GetTriggeredEvent(this);

        if (effects.Any() == false && triggeredEvent == null)
        {
            writer.Indent().WriteLine("# no effect");
            return;
        }

        writer.Indent().WriteLine("effect = {");
        ParadoxText.IndentLevel++;

        if (effects.Any() == false)
        {
            writer.WriteLine();
            writer.Indent().WriteLine("# no scripted effects");
        }
        else
        {
            writer.WriteLine();
            writer.Indent().WriteLine("# scripted effects");

            foreach (Effect eff in effects)
            {
                writer.Indent().WriteLine($"{modService.GetPrefix()}_{eff.Name} = yes");
            }
        }

        writer.WriteLine();

        WriteTriggeredEvent(writer, modService, decisionService);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteTriggeredEvent(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var triggeredEvent = decisionService.GetTriggeredEvent(this);
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

        writer.Indent().WriteLine($"trigger_event = {{ id = {modService.GetPrefix()}.{triggeredEvent.Id} }}");

        if (TriggeredEventScope != string.Empty)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }

    private void WriteAiChance(
        TextWriter writer)
    {
        writer.Indent().WriteLine($"ai_goal = {(AiGoal ? "yes" : "no")}");
        writer.Indent().WriteLine($"ai_check_frequency = {AiCheckFrequency}");

        writer.WriteLine();

        writer.Indent().WriteLine("ai_potential = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"always = {(AiPotential ? "yes" : "no")}");

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");

        writer.WriteLine();

        writer.Indent().WriteLine("ai_will_do = {");
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

    public void WriteLoc(
        TextWriter writer,
        IModService modService)
    {
        writer.WriteLocLine($"{modService.GetPrefix()}.{Name}.t", Title);
        writer.WriteLocLine($"{modService.GetPrefix()}.{Name}.d", Description);
        writer.WriteLocLine($"{modService.GetPrefix()}.{Name}.tt", Tooltip);
        writer.WriteLocLine($"{modService.GetPrefix()}.{Name}.c", Confirm);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Decision);
    }

    public bool Equals(Decision? other)
    {
        return other is not null &&
               id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }

    public static bool operator ==(Decision? left, Decision? right)
    {
        return EqualityComparer<Decision>.Default.Equals(left, right);
    }

    public static bool operator !=(Decision? left, Decision? right)
    {
        return !(left == right);
    }
}

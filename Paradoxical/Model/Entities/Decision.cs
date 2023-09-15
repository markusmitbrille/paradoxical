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

[Table("decisions")]
public class Decision : IEntity, IModel, IElement, IEquatable<Decision?>, IComparable<Decision>
{
    [Column("id"), PrimaryKey, AutoIncrement]
    public int Id { get => id; set => id = value; }
    public int id;

    [Column("name"), Indexed, NotNull]
    public string Name { get => name; set => name = value; }
    public string name = $"dec_{Guid.NewGuid().ToString("N").Substring(0, 4)}";

    [Column("title"), NotNull]
    public string Title { get => title; set => title = value; }
    public string title = "";

    [Column("description"), NotNull]
    public string Description { get => description; set => description = value; }
    public string description = "";

    [Column("confirm"), NotNull]
    public string Confirm { get => confirm; set => confirm = value; }
    public string confirm = "";

    [Column("tooltip"), NotNull]
    public string Tooltip { get => tooltip; set => tooltip = value; }
    public string tooltip = "";

    [Column("picture"), NotNull]
    public string Picture { get => picture; set => picture = value; }
    public string picture = "";

    [Column("cooldown"), NotNull]
    public int Cooldown { get => cooldown; set => cooldown = value; }
    public int cooldown;

    [Column("major"), NotNull]
    public bool Major { get => major; set => major = value; }
    public bool major;

    [Column("order"), NotNull]
    public int Order { get => order; set => order = value; }
    public int order;

    [Column("cost_gold"), NotNull]
    public int CostGold { get => costGold; set => costGold = value; }
    public int costGold;

    [Column("cost_piety"), NotNull]
    public int CostPiety { get => costPiety; set => costPiety = value; }
    public int costPiety;

    [Column("cost_prestige"), NotNull]
    public int CostPrestige { get => costPrestige; set => costPrestige = value; }
    public int costPrestige;

    [Column("min_cost_gold"), NotNull]
    public int MinCostGold { get => minCostGold; set => minCostGold = value; }
    public int minCostGold;

    [Column("min_cost_piety"), NotNull]
    public int MinCostPiety { get => minCostPiety; set => minCostPiety = value; }
    public int minCostPiety;

    [Column("min_cost_prestige"), NotNull]
    public int MinCostPrestige { get => minCostPrestige; set => minCostPrestige = value; }
    public int minCostPrestige;

    [Column("custom_shown_trigger"), NotNull]
    public string CustomShownTrigger { get => customShownTrigger; set => customShownTrigger = value; }
    public string customShownTrigger = "";

    [Column("custom_failure_trigger"), NotNull]
    public string CustomFailureTrigger { get => customFailureTrigger; set => customFailureTrigger = value; }
    public string customFailureTrigger = "";

    [Column("custom_valid_trigger"), NotNull]
    public string CustomValidTrigger { get => customValidTrigger; set => customValidTrigger = value; }
    public string customValidTrigger = "";

    [Column("custom_effect"), NotNull]
    public string CustomEffect { get => customEffect; set => customEffect = value; }
    public string customEffect = "";

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

    [Column("ai_goal"), NotNull]
    public bool AiGoal { get => aiGoal; set => aiGoal = value; }
    public bool aiGoal;

    [Column("ai_check_frequency"), NotNull]
    public int AiCheckFrequency { get => aiCheckFrequency; set => aiCheckFrequency = value; }
    public int aiCheckFrequency;

    [Column("ai_potential"), NotNull]
    public string AiPotential { get => aiPotential; set => aiPotential = value; }
    public string aiPotential = "";

    [Column("ai_base_will_do"), NotNull]
    public int AiBaseWillDo { get => aiBaseWillDo; set => aiBaseWillDo = value; }
    public int aiBaseWillDo;

    [Column("ai_custom_will_do"), NotNull]
    public string AiCustomWillDo { get => aiCustomWillDo; set => aiCustomWillDo = value; }
    public string aiCustomWillDo = "";

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

    public Decision()
    {
    }

    public Decision(Decision other)
    {
        id = 0;

        name = other.name;
        title = other.title;
        description = other.description;
        confirm = other.confirm;
        tooltip = other.tooltip;
        picture = other.picture;

        major = other.major;
        order = other.order;

        costGold = other.costGold;
        costPiety = other.costPiety;
        costPrestige = other.costPrestige;

        minCostGold = other.minCostGold;
        minCostPiety = other.minCostPiety;
        minCostPrestige = other.minCostPrestige;

        customShownTrigger = other.customShownTrigger;
        customFailureTrigger = other.customFailureTrigger;
        customValidTrigger = other.customValidTrigger;
        customEffect = other.customEffect;

        triggeredEventId = other.triggeredEventId;
        triggeredEventScope = other.triggeredEventScope;
        triggeredEventMinDays = other.triggeredEventMinDays;
        triggeredEventMaxDays = other.triggeredEventMaxDays;

        aiGoal = other.aiGoal;
        aiCheckFrequency = other.aiCheckFrequency;

        aiPotential = other.aiPotential;

        aiBaseWillDo = other.aiBaseWillDo;
        aiCustomWillDo = other.aiCustomWillDo;

        aiBoldnessTargetModifier = other.aiBoldnessTargetModifier;
        aiCompassionTargetModifier = other.aiCompassionTargetModifier;
        aiGreedTargetModifier = other.aiGreedTargetModifier;
        aiEnergyTargetModifier = other.aiEnergyTargetModifier;
        aiHonorTargetModifier = other.aiHonorTargetModifier;
        aiRationalityTargetModifier = other.aiRationalityTargetModifier;
        aiSociabilityTargetModifier = other.aiSociabilityTargetModifier;
        aiVengefulnessTargetModifier = other.aiVengefulnessTargetModifier;
        aiZealTargetModifier = other.aiZealTargetModifier;
    }

    public string GetQualifiedName(IModService modService)
    {
        return $"{modService.GetPrefix()}_dec_{Id}";
    }

    public string GetLocationKey(IModService modService)
    {
        return $"{modService.GetPrefix()}.decision.{Id}";
    }

    public void Write(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        writer.Indent().WriteLine($"{GetQualifiedName(modService)} = {{");
        ParadoxText.IndentLevel++;

        writer.WriteLine();

        writer.Indent().WriteLine($"major = {(Major ? "yes" : "no")}");
        writer.Indent().WriteLine($"sort_order = {Order}");

        writer.WriteLine();

        writer.Indent().WriteLine($"title = {GetLocationKey(modService)}.t");
        writer.Indent().WriteLine($"desc = {GetLocationKey(modService)}.d");
        writer.Indent().WriteLine($"confirm_text = {GetLocationKey(modService)}.ct");
        writer.Indent().WriteLine($"selection_tooltip = {GetLocationKey(modService)}.tt");

        writer.WriteLine();
        WritePicture(writer);

        writer.WriteLine();
        WriteCooldown(writer);

        writer.WriteLine();
        WriteCost(writer);

        writer.WriteLine();
        WriteMinCost(writer);

        writer.WriteLine();
        WriteShown(writer, modService, decisionService);

        writer.WriteLine();
        WriteFailure(writer, modService, decisionService);

        writer.WriteLine();
        WriteValid(writer, modService, decisionService);

        writer.WriteLine();
        WriteEffect(writer, modService, decisionService);

        writer.WriteLine();
        writer.Indent().WriteLine($"ai_goal = {(AiGoal ? "yes" : "no")}");
        writer.Indent().WriteLine($"ai_check_interval = {AiCheckFrequency}");

        writer.WriteLine();
        WriteAiPotential(writer, modService, decisionService);

        writer.WriteLine();
        WriteAiWillDo(writer);

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WritePicture(
        TextWriter writer)
    {
        if (Picture.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no picture");
            return;
        }

        writer.Indent().WriteLine($"picture = \"{Picture}\"");
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

    private void WriteCost(TextWriter writer)
    {
        if (CostGold <= 0 && CostPiety <= 0 && CostPrestige <= 0)
        {
            writer.Indent().WriteLine("# no cost");
            return;
        }

        writer.Indent().WriteLine("cost = {");
        ParadoxText.IndentLevel++;

        if (CostGold > 0)
        {
            writer.Indent().WriteLine($"gold = {CostGold}");
        }
        if (CostPiety > 0)
        {
            writer.Indent().WriteLine($"piety = {CostPiety}");
        }
        if (CostPrestige > 0)
        {
            writer.Indent().WriteLine($"prestige = {CostPrestige}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteMinCost(TextWriter writer)
    {
        if (MinCostGold <= 0 && MinCostPiety <= 0 && MinCostPrestige <= 0)
        {
            writer.Indent().WriteLine("# no min cost");
            return;
        }

        writer.Indent().WriteLine("minimum_cost = {");
        ParadoxText.IndentLevel++;

        if (MinCostGold > 0)
        {
            writer.Indent().WriteLine($"gold = {CostGold}");
        }
        if (MinCostPiety > 0)
        {
            writer.Indent().WriteLine($"piety = {CostPiety}");
        }
        if (MinCostPrestige > 0)
        {
            writer.Indent().WriteLine($"prestige = {CostPrestige}");
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteShown(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        if (CustomShownTrigger.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no is-shown trigger");
            return;
        }

        writer.Indent().WriteLine("is_shown = {");
        ParadoxText.IndentLevel++;

        if (CustomShownTrigger.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom trigger");

            foreach (string line in CustomShownTrigger.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteFailure(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        if (CustomFailureTrigger.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no is-valid trigger (showing failures only)");
            return;
        }

        writer.Indent().WriteLine("is_valid_showing_failures_only = {");
        ParadoxText.IndentLevel++;

        if (CustomFailureTrigger.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom trigger");

            foreach (string line in CustomFailureTrigger.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteValid(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        if (CustomValidTrigger.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no is-valid trigger");
            return;
        }

        writer.Indent().WriteLine("is_valid = {");
        ParadoxText.IndentLevel++;

        if (CustomValidTrigger.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom trigger");

            foreach (string line in CustomValidTrigger.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteEffect(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        var triggeredEvent = decisionService.GetTriggeredEvent(this);

        if (CustomEffect.IsEmpty() == true && triggeredEvent == null)
        {
            writer.Indent().WriteLine("# no effect");
            return;
        }

        writer.Indent().WriteLine("effect = {");
        ParadoxText.IndentLevel++;

        WriteTriggeredEvent(writer, modService, decisionService);

        if (CustomEffect.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom effect");

            foreach (string line in CustomEffect.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

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

        writer.Indent().WriteLine($"id = {triggeredEvent.GetQualifiedName(modService)}");
        writer.Indent().WriteLine($"days = {{ {TriggeredEventMinDays} {TriggeredEventMaxDays} }}");

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");

        if (TriggeredEventScope != string.Empty)
        {
            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }

    private void WriteAiPotential(
        TextWriter writer,
        IModService modService,
        IDecisionService decisionService)
    {
        if (AiPotential.IsEmpty() == true)
        {
            writer.Indent().WriteLine("# no ai potential");
            return;
        }

        writer.Indent().WriteLine("ai_potential = {");
        ParadoxText.IndentLevel++;

        if (AiPotential.IsEmpty() == false)
        {
            writer.Indent().WriteLine("# custom trigger");

            foreach (string line in AiPotential.Split(ParadoxText.NewParagraph))
            {
                writer.Indent().WriteLine(line);
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    private void WriteAiWillDo(TextWriter writer)
    {
        writer.Indent().WriteLine("ai_will_do = {");
        ParadoxText.IndentLevel++;

        writer.Indent().WriteLine($"base = {AiBaseWillDo}");

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

            if (AiCustomWillDo.IsEmpty() == false)
            {
                writer.Indent().WriteLine("# custom ai will-do");

                foreach (string line in AiCustomWillDo.Split(ParadoxText.NewParagraph))
                {
                    writer.Indent().WriteLine(line);
                }
            }
        }

        ParadoxText.IndentLevel--;
        writer.Indent().WriteLine("}");
    }

    public void WriteLoc(
        TextWriter writer,
        IModService modService)
    {
        writer.WriteLocLine($"{GetLocationKey(modService)}.t", Title);
        writer.WriteLocLine($"{GetLocationKey(modService)}.d", Description);
        writer.WriteLocLine($"{GetLocationKey(modService)}.ct", Confirm);
        writer.WriteLocLine($"{GetLocationKey(modService)}.tt", Tooltip);
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

    public int CompareTo(Decision? other)
    {
        return Comparer<string?>.Default.Compare(name, other?.name);
    }

    public static bool operator ==(Decision? left, Decision? right)
    {
        return EqualityComparer<Decision>.Default.Equals(left, right);
    }

    public static bool operator !=(Decision? left, Decision? right)
    {
        return !(left == right);
    }

    public static bool operator <(Decision left, Decision right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Decision left, Decision right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Decision left, Decision right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Decision left, Decision right)
    {
        return left.CompareTo(right) >= 0;
    }
}

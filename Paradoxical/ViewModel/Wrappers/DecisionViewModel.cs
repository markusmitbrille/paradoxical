using Paradoxical.Core;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public class DecisionViewModel : ElementWrapper<Decision>, IEquatable<DecisionViewModel?>
{
    public override string Kind => "decision";

    public override string Name
    {
        get => model.name;
        set => SetProperty(ref model.name, value);
    }

    public string Title
    {
        get => model.title;
        set => SetProperty(ref model.title, value);
    }

    public string Description
    {
        get => model.description;
        set => SetProperty(ref model.description, value);
    }

    public string Confirm
    {
        get => model.confirm;
        set => SetProperty(ref model.confirm, value);
    }

    public string Tooltip
    {
        get => model.tooltip;
        set => SetProperty(ref model.tooltip, value);
    }

    public string Picture
    {
        get => model.picture;
        set => SetProperty(ref model.picture, value);
    }

    public int Cooldown
    {
        get => model.cooldown;
        set => SetProperty(ref model.cooldown, value);
    }

    public bool Major
    {
        get => model.major;
        set => SetProperty(ref model.major, value);
    }

    public int Order
    {
        get => model.order;
        set => SetProperty(ref model.order, value);
    }

    public int CostGold
    {
        get => model.costGold;
        set => SetProperty(ref model.costGold, value);
    }

    public int CostPiety
    {
        get => model.costPiety;
        set => SetProperty(ref model.costPiety, value);
    }

    public int CostPrestige
    {
        get => model.costPrestige;
        set => SetProperty(ref model.costPrestige, value);
    }

    public int MinCostGold
    {
        get => model.minCostGold;
        set => SetProperty(ref model.minCostGold, value);
    }

    public int MinCostPiety
    {
        get => model.minCostPiety;
        set => SetProperty(ref model.minCostPiety, value);
    }

    public int MinCostPrestige
    {
        get => model.minCostPrestige;
        set => SetProperty(ref model.minCostPrestige, value);
    }

    public string CustomShownTrigger
    {
        get => model.customShownTrigger;
        set => SetProperty(ref model.customShownTrigger, value);
    }

    public string CustomFailureTrigger
    {
        get => model.customFailureTrigger;
        set => SetProperty(ref model.customFailureTrigger, value);
    }

    public string CustomValidTrigger
    {
        get => model.customValidTrigger;
        set => SetProperty(ref model.customValidTrigger, value);
    }

    public string CustomEffect
    {
        get => model.customEffect;
        set => SetProperty(ref model.customEffect, value);
    }

    public int? TriggeredEventId
    {
        get => model.triggeredEventId;
        set => SetProperty(ref model.triggeredEventId, value);
    }

    public string TriggeredEventScope
    {
        get => model.triggeredEventScope;
        set => SetProperty(ref model.triggeredEventScope, value);
    }

    public int TriggeredEventMinDays
    {
        get => model.triggeredEventMinDays;
        set => SetProperty(ref model.triggeredEventMinDays, value);
    }

    public int TriggeredEventMaxDays
    {
        get => model.triggeredEventMaxDays;
        set => SetProperty(ref model.triggeredEventMaxDays, value);
    }

    public bool AiGoal
    {
        get => model.aiGoal;
        set => SetProperty(ref model.aiGoal, value);
    }

    public int AiCheckFrequency
    {
        get => model.aiCheckFrequency;
        set => SetProperty(ref model.aiCheckFrequency, value);
    }

    public string AiPotential
    {
        get => model.aiPotential;
        set => SetProperty(ref model.aiPotential, value);
    }

    public int AiBaseWillDo
    {
        get => model.aiBaseWillDo;
        set => SetProperty(ref model.aiBaseWillDo, value);
    }

    public string AiCustomWillDo
    {
        get => model.aiCustomWillDo;
        set => SetProperty(ref model.aiCustomWillDo, value);
    }

    public int AiBoldnessTargetModifier
    {
        get => model.aiBoldnessTargetModifier;
        set => SetProperty(ref model.aiBoldnessTargetModifier, value);
    }

    public int AiCompassionTargetModifier
    {
        get => model.aiCompassionTargetModifier;
        set => SetProperty(ref model.aiCompassionTargetModifier, value);
    }

    public int AiGreedTargetModifier
    {
        get => model.aiGreedTargetModifier;
        set => SetProperty(ref model.aiGreedTargetModifier, value);
    }

    public int AiEnergyTargetModifier
    {
        get => model.aiEnergyTargetModifier;
        set => SetProperty(ref model.aiEnergyTargetModifier, value);
    }

    public int AiHonorTargetModifier
    {
        get => model.aiHonorTargetModifier;
        set => SetProperty(ref model.aiHonorTargetModifier, value);
    }

    public int AiRationalityTargetModifier
    {
        get => model.aiRationalityTargetModifier;
        set => SetProperty(ref model.aiRationalityTargetModifier, value);
    }

    public int AiSociabilityTargetModifier
    {
        get => model.aiSociabilityTargetModifier;
        set => SetProperty(ref model.aiSociabilityTargetModifier, value);
    }

    public int AiVengefulnessTargetModifier
    {
        get => model.aiVengefulnessTargetModifier;
        set => SetProperty(ref model.aiVengefulnessTargetModifier, value);
    }

    public int AiZealTargetModifier
    {
        get => model.aiZealTargetModifier;
        set => SetProperty(ref model.aiZealTargetModifier, value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DecisionViewModel);
    }

    public bool Equals(DecisionViewModel? other)
    {
        return other is not null &&
               EqualityComparer<Decision>.Default.Equals(model, other.model);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(model);
    }

    public static bool operator ==(DecisionViewModel? left, DecisionViewModel? right)
    {
        return EqualityComparer<DecisionViewModel>.Default.Equals(left, right);
    }

    public static bool operator !=(DecisionViewModel? left, DecisionViewModel? right)
    {
        return !(left == right);
    }
}

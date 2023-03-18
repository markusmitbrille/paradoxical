using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.ViewModel;

public class DecisionViewModel : ElementViewModel<Decision>
{
    public string Name
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

    public string Tooltip
    {
        get => model.tooltip;
        set => SetProperty(ref model.tooltip, value);
    }

    public string Confirm
    {
        get => model.confirm;
        set => SetProperty(ref model.confirm, value);
    }

    public string Picture
    {
        get => model.picture;
        set => SetProperty(ref model.picture, value);
    }

    public bool Major
    {
        get => model.major;
        set => SetProperty(ref model.major, value);
    }

    public int SortOrder
    {
        get => model.sortOrder;
        set => SetProperty(ref model.sortOrder, value);
    }

    public int Cooldown
    {
        get => model.cooldown;
        set => SetProperty(ref model.cooldown, value);
    }

    public int GoldCost
    {
        get => model.goldCost;
        set => SetProperty(ref model.goldCost, value);
    }

    public int PietyCost
    {
        get => model.pietyCost;
        set => SetProperty(ref model.pietyCost, value);
    }

    public int PrestigeCost
    {
        get => model.prestigeCost;
        set => SetProperty(ref model.prestigeCost, value);
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

    public bool AiPotential
    {
        get => model.aiPotential;
        set => SetProperty(ref model.aiPotential, value);
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

    public int AiBaseChance
    {
        get => model.aiBaseChance;
        set => SetProperty(ref model.aiBaseChance, value);
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

    public DecisionViewModel(Decision model) : base(model)
    {
    }
}

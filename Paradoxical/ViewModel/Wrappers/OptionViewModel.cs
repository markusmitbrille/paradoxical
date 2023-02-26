using Paradoxical.Core;
using Paradoxical.Model;

namespace Paradoxical.ViewModel;

public partial class OptionViewModel : ModelViewModelBase
{
    private readonly Option model;
    public Option Model => model;

    public int Id
    {
        get => model.Id;
    }

    public int EventId
    {
        get => model.eventId;
    }

    public string Title
    {
        get => model.title;
        set => SetProperty(ref model.title, value);
    }

    public string Tooltip
    {
        get => model.tooltip;
        set => SetProperty(ref model.tooltip, value);
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

    public OptionViewModel(Option model)
    {
        this.model = model;
    }
}
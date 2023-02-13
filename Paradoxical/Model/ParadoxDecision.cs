using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxDecision : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string description = "";
        [ObservableProperty]
        private string tooltip = "";
        [ObservableProperty]
        private string confirm = "";
        [ObservableProperty]
        private string picture = "";

        [ObservableProperty]
        private bool major = false;
        [ObservableProperty]
        private int sortOrder = 0;

        [ObservableProperty]
        private int goldCost;
        [ObservableProperty]
        private int pietyCost;
        [ObservableProperty]
        private int prestigeCost;

        [ObservableProperty]
        private string isShownTrigger = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> isShownTriggers = new();

        [ObservableProperty]
        private string isValidTrigger = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> isValidTriggers = new();

        [ObservableProperty]
        private string effect = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> effects = new();

        [ObservableProperty]
        private bool aiGoal = false;
        [ObservableProperty]
        private int aiCheckFrequency = 0;

        [ObservableProperty]
        private string aiPotentialTrigger = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> aiPotentialTriggers = new();

        [ObservableProperty]
        private int aiBaseChance = 0;

        // up to this much is added or subtracted based on personality
        [ObservableProperty]
        private int aiBoldnessTargetModifier = 0;
        [ObservableProperty]
        private int aiCompassionTargetModifier = 0;
        [ObservableProperty]
        private int aiGreedTargetModifier = 0;
        [ObservableProperty]
        private int aiEnergyTargetModifier = 0;
        [ObservableProperty]
        private int aiHonorTargetModifier = 0;
        [ObservableProperty]
        private int aiRationalityTargetModifier = 0;
        [ObservableProperty]
        private int aiSociabilityTargetModifier = 0;
        [ObservableProperty]
        private int aiVengefulnessTargetModifier = 0;
        [ObservableProperty]
        private int aiZealTargetModifier = 0;

        [ObservableProperty]
        private string aiChance = "";

        public ParadoxDecision() : base()
        {
            name = $"decision_{Guid.NewGuid().ToString()[0..4]}";

            title = "Hello World";
            description = "Hello World!";
            tooltip = "Hello World";
            confirm = "Hello World";
        }

        public ParadoxDecision(ParadoxDecision other) : this()
        {
            title = other.title;
            description = other.description;
            tooltip = other.tooltip;
            confirm = other.confirm;
            picture = other.picture;

            major = other.major;
            sortOrder = other.sortOrder;

            goldCost = other.goldCost;
            pietyCost = other.pietyCost;
            prestigeCost = other.prestigeCost;

            isShownTrigger = other.isShownTrigger;
            isValidTrigger = other.isValidTrigger;
            effect = other.effect;

            // aggregate association, therefore shallow copy
            isShownTriggers = new(other.isShownTriggers);
            isValidTriggers = new(other.isValidTriggers);
            effects = new(other.effects);

            aiGoal = other.aiGoal;
            aiCheckFrequency = other.aiCheckFrequency;

            aiPotentialTrigger = other.aiPotentialTrigger;

            aiBaseChance = other.aiBaseChance;

            // aggregate association, therefore shallow copy
            aiPotentialTriggers = new(other.aiPotentialTriggers);

            aiBoldnessTargetModifier = other.aiBoldnessTargetModifier;
            aiCompassionTargetModifier = other.aiCompassionTargetModifier;
            aiGreedTargetModifier = other.aiGreedTargetModifier;
            aiEnergyTargetModifier = other.aiEnergyTargetModifier;
            aiHonorTargetModifier = other.aiHonorTargetModifier;
            aiRationalityTargetModifier = other.aiRationalityTargetModifier;
            aiSociabilityTargetModifier = other.aiSociabilityTargetModifier;
            aiVengefulnessTargetModifier = other.aiVengefulnessTargetModifier;
            aiZealTargetModifier = other.aiZealTargetModifier;

            aiChance = other.aiChance;
        }

        public void Write(TextWriter writer)
        {
        }

        public void WriteLoc(TextWriter writer)
        {
        }
    }
}

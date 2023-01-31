using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.Model
{
    public partial class ParadoxEventOption : ObservableObject
    {
        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string tooltip = "";

        [ObservableProperty]
        private ParadoxEvent? triggerEvent = null;
        [ObservableProperty]
        private string triggerEventScope = "";

        [ObservableProperty]
        private string trigger = "";
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();

        [ObservableProperty]
        private string effect = "";
        public ObservableCollection<ParadoxEffect> Effects { get; } = new();

        [ObservableProperty]
        private int aiBaseChance = 100;

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
    }
}
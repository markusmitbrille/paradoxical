using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxEventOptionViewModel : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string tooltip = "";

        [ObservableProperty]
        private ParadoxEventViewModel? triggeredEvent = null;
        [ObservableProperty]
        private string triggeredEventScope = "";

        [ObservableProperty]
        private string trigger = "";
        public ObservableCollection<ParadoxTriggerViewModel> Triggers { get; } = new();

        [ObservableProperty]
        private string effect = "";
        public ObservableCollection<ParadoxEffectViewModel> Effects { get; } = new();

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

        public ParadoxEventOptionViewModel(ModContext context)
        {
            Context = context;
        }

        [RelayCommand]
        private async void AddTrigger()
        {
            // TODO: somehow get the necessary data ... services? dependency injection? voodoo?

            FindTriggerDialogViewModel vm = new()
            {
                //Items = ActiveMod.Triggers,
            };
            FindTriggerDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            Triggers.Add(vm.Selected);
        }

        [RelayCommand]
        private void RemoveTrigger(ParadoxTriggerViewModel trg)
        {
            Triggers.Remove(trg);
        }
    }
}
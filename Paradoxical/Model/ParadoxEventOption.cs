using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace Paradoxical.Model
{
    public partial class ParadoxEventOption : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string tooltip = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateTriggeredEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggeredEventCommand))]
        private ParadoxEvent? triggeredEvent = null;
        [ObservableProperty]
        private string triggeredEventScope = "";

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

        public ParadoxEventOption(ModContext context)
        {
            Context = context;

            name = $"Option [{Guid.NewGuid().ToString()[0..4]}]";
        }

        public ParadoxEventOption(ModContext context, ParadoxEventOption other) : this(context)
        {
            name = other.name;
            tooltip = other.tooltip;

            triggeredEvent = other.triggeredEvent;
            triggeredEventScope = other.triggeredEventScope;

            trigger = other.trigger;
            effect = other.effect;

            // aggregate association, therefore shallow copy
            Triggers = new(other.Triggers);
            Effects = new(other.Effects);

            aiBaseChance = other.aiBaseChance;
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

        [RelayCommand]
        private void CreateTrigger()
        {
            ParadoxTrigger trg = new(Context);

            Context.Triggers.Add(trg);
            Triggers.Add(trg);
        }

        [RelayCommand(CanExecute = nameof(CanRemoveTrigger))]
        private void RemoveTrigger(ParadoxTrigger trg)
        {
            Triggers.Remove(trg);
        }
        private bool CanRemoveTrigger(ParadoxTrigger trg)
        {
            return Triggers.Contains(trg);
        }

        [RelayCommand]
        private async void FindTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Triggers,
                Blacklist = new(Triggers),
            };
            FindTriggerDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            Triggers.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateEffect()
        {
            ParadoxEffect eff = new(Context);

            Context.Effects.Add(eff);
            Effects.Add(eff);
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEffect))]
        private void RemoveEffect(ParadoxEffect eff)
        {
            Effects.Remove(eff);
        }
        private bool CanRemoveEffect(ParadoxEffect eff)
        {
            return Effects.Contains(eff);
        }

        [RelayCommand]
        private async void FindEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Effects,
                Blacklist = new(Effects),
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            Effects.Add(vm.Selected);
        }

        [RelayCommand(CanExecute = nameof(CanCreateTriggeredEvent))]
        private void CreateTriggeredEvent()
        {
            ParadoxEvent evt = new(Context);

            Context.Events.Add(evt);
            TriggeredEvent = evt;
        }
        private bool CanCreateTriggeredEvent()
        {
            return TriggeredEvent == null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveTriggeredEvent))]
        private void RemoveTriggeredEvent()
        {
            TriggeredEvent = null;
        }
        private bool CanRemoveTriggeredEvent()
        {
            return TriggeredEvent != null;
        }

        [RelayCommand]
        private async void FindTriggeredEvent()
        {
            FindEventDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Events,
                Selected = TriggeredEvent,
            };
            FindEventDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            TriggeredEvent = vm.Selected;
        }
    }
}
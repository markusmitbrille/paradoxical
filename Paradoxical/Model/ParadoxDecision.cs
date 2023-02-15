using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;
using Paradoxical.ViewModel;
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
        [NotifyCanExecuteChangedFor(nameof(CreateTriggeredEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggeredEventCommand))]
        private ParadoxEvent? triggeredEvent = null;
        [ObservableProperty]
        private string triggeredEventScope = "";

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
        private bool aiPotential = true;
        [ObservableProperty]
        private bool aiGoal = false;
        [ObservableProperty]
        private int aiCheckFrequency = 0;

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

            triggeredEvent = other.triggeredEvent;
            triggeredEventScope = other.triggeredEventScope;

            isShownTrigger = other.isShownTrigger;
            isValidTrigger = other.isValidTrigger;
            effect = other.effect;

            // aggregate association, therefore shallow copy
            isShownTriggers = new(other.isShownTriggers);
            isValidTriggers = new(other.isValidTriggers);
            effects = new(other.effects);

            aiGoal = other.aiGoal;
            aiCheckFrequency = other.aiCheckFrequency;

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
        private void CreateIsShownTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            IsShownTriggers.Add(trg);
        }

        [RelayCommand]
        private void RemoveIsShownTrigger(ParadoxTrigger trg)
        {
            IsShownTriggers.Remove(trg);
        }

        [RelayCommand]
        private async void FindIsShownTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Triggers,
                Blacklist = new(IsShownTriggers),
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

            IsShownTriggers.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateIsValidTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            IsValidTriggers.Add(trg);
        }

        [RelayCommand]
        private void RemoveIsValidTrigger(ParadoxTrigger trg)
        {
            IsValidTriggers.Remove(trg);
        }

        [RelayCommand]
        private async void FindIsValidTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Triggers,
                Blacklist = new(IsValidTriggers),
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

            IsValidTriggers.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateEffect()
        {
            ParadoxEffect eff = new();

            Context.Current.Effects.Add(eff);
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
                Items = Context.Current.Effects,
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
            ParadoxEvent evt = new();

            Context.Current.Events.Add(evt);
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
                Items = Context.Current.Events,
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

        public void Write(TextWriter writer)
        {
        }

        public void WriteLoc(TextWriter writer)
        {
        }
    }
}

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
    public partial class ParadoxEventOption : ObservableObject
    {
        public Context CurrentContext => Context.Current;

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
        private int triggeredEventMinDays = 0;
        [ObservableProperty]
        private int triggeredEventMaxDays = 0;

        [ObservableProperty]
        private string trigger = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> triggers = new();

        [ObservableProperty]
        private string effect = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> effects = new();

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

        public ParadoxEventOption()
        {
            name = $"New Option";
        }

        public ParadoxEventOption(ParadoxEventOption other) : this()
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
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
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
                Items = Context.Current.Triggers,
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

        public void Write(TextWriter writer, ParadoxEvent parent, int index)
        {
            writer.Indent().WriteLine("option = {");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"name = {Context.Current.Info.EventNamespace}.{parent.Id}.o.{index}");
            WriteTooltip(writer, parent, index);

            writer.WriteLine();

            WriteTrigger(writer);

            writer.WriteLine();

            WriteAiChance(writer);

            writer.WriteLine();

            WriteTriggeredEvent(writer);

            writer.WriteLine();

            WriteEffect(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteTooltip(TextWriter writer, ParadoxEvent parent, int index)
        {
            if (Tooltip == string.Empty)
            {
                writer.Indent().WriteLine("# no custom tooltip");
                return;
            }

            writer.Indent().WriteLine($"custom_tooltip = {Context.Current.Info.EventNamespace}.{parent.Id}.o.{index}.tt");
        }

        private void WriteTrigger(TextWriter writer)
        {
            if (Trigger == string.Empty && Triggers.Count == 0)
            {
                writer.Indent().WriteLine("# no trigger");
                return;
            }

            writer.Indent().WriteLine("trigger = {");
            ParadoxText.IndentLevel++;

            if (Trigger == string.Empty)
            {
                writer.Indent().WriteLine("# no custom trigger");
            }
            else
            {
                foreach (string line in Trigger.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            if (Triggers.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted triggers");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted triggers");

                foreach (ParadoxTrigger trg in Triggers)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{trg.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteAiChance(TextWriter writer)
        {
            writer.Indent().WriteLine("ai_chance = {");
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

            if (AiChance == string.Empty)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no custom ai chance");
            }
            else
            {
                writer.WriteLine();
                foreach (string line in AiChance.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteTriggeredEvent(TextWriter writer)
        {
            if (TriggeredEvent == null)
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

            writer.Indent().WriteLine("trigger_event = {");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"id = {Context.Current.Info.EventNamespace}.{TriggeredEvent.Id}");
            writer.Indent().WriteLine($"days = {{ {TriggeredEventMinDays} {TriggeredEventMaxDays} }}");

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");

            if (TriggeredEventScope != string.Empty)
            {
                ParadoxText.IndentLevel--;
                writer.Indent().WriteLine("}");
            }
        }

        private void WriteEffect(TextWriter writer)
        {
            if (Effect == string.Empty && Effects.Count == 0)
            {
                writer.Indent().WriteLine("# no special effect");
                return;
            }

            if (Effect == string.Empty)
            {
                writer.Indent().WriteLine("# no custom effect");
            }
            else
            {
                foreach (string line in Effect.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            if (Effects.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted effects");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted effects");

                foreach (ParadoxEffect eff in Effects)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{eff.Name} = yes");
                }
            }
        }

        public void WriteLoc(TextWriter writer, ParadoxEvent parent, int index)
        {
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{parent.Id}.o.{index}", Name);
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{parent.Id}.o.{index}.tt", Tooltip);
        }
    }
}
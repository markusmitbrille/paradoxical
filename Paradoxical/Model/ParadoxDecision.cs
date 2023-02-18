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
        private const string DEFAULT_PICTURE = "gfx/interface/illustrations/decisions/decision_misc.dds";

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
        private int cooldown = 0;

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
        private string isValidFailureTrigger = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> isValidFailureTriggers = new();

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
        private void CreateIsValidFailureTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            IsValidFailureTriggers.Add(trg);
        }

        [RelayCommand]
        private void RemoveIsValidFailureTrigger(ParadoxTrigger trg)
        {
            IsValidFailureTriggers.Remove(trg);
        }

        [RelayCommand]
        private async void FindIsValidFailureTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Triggers,
                Blacklist = new(IsValidFailureTriggers),
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

            IsValidFailureTriggers.Add(vm.Selected);
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
            writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{Name} = {{");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"title = {Context.Current.Info.EventNamespace}.{Name}.t");
            writer.Indent().WriteLine($"desc = {Context.Current.Info.EventNamespace}.{Name}.d");
            writer.Indent().WriteLine($"selection_tooltip = {Context.Current.Info.EventNamespace}.{Name}.tt");
            writer.Indent().WriteLine($"confirm_text = {Context.Current.Info.EventNamespace}.{Name}.c");
            writer.Indent().WriteLine($"picture = \"{(Picture == string.Empty ? DEFAULT_PICTURE : Picture)}\"");

            writer.WriteLine();

            writer.Indent().WriteLine($"major = {(Major ? "yes" : "no")}");
            writer.Indent().WriteLine($"sort_order = {SortOrder}");

            writer.WriteLine();
            WriteCooldown(writer);

            writer.WriteLine();
            WriteCost(writer);

            writer.WriteLine();
            WriteIsShownTrigger(writer);

            writer.WriteLine();
            WriteIsValidTrigger(writer);

            writer.WriteLine();
            WriteIsValidFailureTrigger(writer);

            writer.WriteLine();
            WriteEffect(writer);

            writer.WriteLine();
            WriteAiChance(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteCooldown(TextWriter writer)
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
            if (GoldCost <= 0 && PietyCost <= 0 && PrestigeCost <= 0)
            {
                writer.Indent().WriteLine("# no cost");
                return;
            }

            writer.Indent().WriteLine("cost = {");
            ParadoxText.IndentLevel++;

            if (GoldCost > 0)
            {
                writer.Indent().WriteLine($"gold = {GoldCost}");
            }
            if (PietyCost > 0)
            {
                writer.Indent().WriteLine($"piety = {PietyCost}");
            }
            if (PrestigeCost > 0)
            {
                writer.Indent().WriteLine($"prestige = {PrestigeCost}");
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteIsShownTrigger(TextWriter writer)
        {
            if (IsShownTrigger == string.Empty && IsShownTriggers.Count == 0)
            {
                writer.Indent().WriteLine("# no is-shown");
                return;
            }

            writer.Indent().WriteLine("is_shown = {");
            ParadoxText.IndentLevel++;

            if (IsShownTrigger == string.Empty)
            {
                writer.Indent().WriteLine("# no custom trigger");
            }
            else
            {
                foreach (string line in IsShownTrigger.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            if (IsShownTriggers.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted triggers");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted triggers");

                foreach (ParadoxTrigger trg in IsShownTriggers)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{trg.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteIsValidTrigger(TextWriter writer)
        {
            if (IsValidTrigger == string.Empty && IsValidTriggers.Count == 0)
            {
                writer.Indent().WriteLine("# no is-valid");
                return;
            }

            writer.Indent().WriteLine("is_valid = {");
            ParadoxText.IndentLevel++;

            if (IsValidTrigger == string.Empty)
            {
                writer.Indent().WriteLine("# no custom trigger");
            }
            else
            {
                foreach (string line in IsValidTrigger.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            if (IsValidTriggers.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted triggers");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted triggers");

                foreach (ParadoxTrigger trg in IsValidTriggers)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{trg.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteIsValidFailureTrigger(TextWriter writer)
        {
            if (IsValidFailureTrigger == string.Empty && IsValidFailureTriggers.Count == 0)
            {
                writer.Indent().WriteLine("# no is-valid failure");
                return;
            }

            writer.Indent().WriteLine("is_valid_showing_failures_only = {");
            ParadoxText.IndentLevel++;

            if (IsValidFailureTrigger == string.Empty)
            {
                writer.Indent().WriteLine("# no custom trigger");
            }
            else
            {
                foreach (string line in IsValidFailureTrigger.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }
            }

            if (IsValidFailureTriggers.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted triggers");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted triggers");

                foreach (ParadoxTrigger trg in IsValidFailureTriggers)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{trg.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteEffect(TextWriter writer)
        {
            if (Effect == string.Empty && Effects.Count == 0 && TriggeredEvent == null)
            {
                writer.Indent().WriteLine("# no effect");
                return;
            }

            writer.Indent().WriteLine("effect = {");
            ParadoxText.IndentLevel++;

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


            writer.WriteLine();

            WriteTriggeredEvent(writer);

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

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");

            if (TriggeredEventScope != string.Empty)
            {
                ParadoxText.IndentLevel--;
                writer.Indent().WriteLine("}");
            }
        }

        private void WriteAiChance(TextWriter writer)
        {
            writer.Indent().WriteLine($"ai_goal = {(AiGoal ? "yes" : "no")}");
            writer.Indent().WriteLine($"ai_check_frequency = {AiCheckFrequency}");

            writer.WriteLine();

            writer.Indent().WriteLine("ai_potential = {");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"always = {(AiPotential ? "yes" : "no")}");

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");

            writer.WriteLine();

            writer.Indent().WriteLine("ai_will_do = {");
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

        public void WriteLoc(TextWriter writer)
        {
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Name}.t", Title);
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Name}.d", Description);
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Name}.tt", Tooltip);
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Name}.c", Confirm);
        }
    }
}

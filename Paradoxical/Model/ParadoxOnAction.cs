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
    public partial class ParadoxOnAction : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private bool vanilla = false;

        [ObservableProperty]
        private int chance = 100;

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent> events = new();

        [ObservableProperty]
        private ObservableCollection<ParadoxOnAction> onActions = new();

        [ObservableProperty]
        private string trigger = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> triggers = new();

        [ObservableProperty]
        private string effect = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> effects = new();

        public ParadoxOnAction()
        {
            name = $"OnAction_{Guid.NewGuid().ToString()[0..4]}";
        }

        public ParadoxOnAction(ParadoxOnAction other) : this()
        {
            name = other.name;
            vanilla = other.vanilla;
            chance = other.chance;

            // aggregate association, therefore shallow copy
            Events = new(other.Events);
            OnActions = new(other.OnActions);

            trigger = other.trigger;
            effect = other.effect;

            // aggregate association, therefore shallow copy
            Triggers = new(other.Triggers);
            Effects = new(other.Effects);
        }

        [RelayCommand]
        private void CreateEvent()
        {
            ParadoxEvent trg = new();

            Context.Current.Events.Add(trg);
            Events.Add(trg);
        }

        [RelayCommand]
        private void RemoveEvent(ParadoxEvent evt)
        {
            Events.Remove(evt);
        }

        [RelayCommand]
        private async void FindEvent()
        {
            FindEventDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Events,
                Blacklist = new(Events),
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

            Events.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateOnAction()
        {
            ParadoxOnAction trg = new();

            Context.Current.OnActions.Add(trg);
            OnActions.Add(trg);
        }

        [RelayCommand]
        private void RemoveOnAction(ParadoxOnAction trg)
        {
            OnActions.Remove(trg);
        }

        [RelayCommand]
        private async void FindOnAction()
        {
            FindOnActionDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.OnActions,
                Blacklist = new(OnActions),
            };
            FindOnActionDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            OnActions.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            Triggers.Add(trg);
        }

        [RelayCommand]
        private void RemoveTrigger(ParadoxTrigger trg)
        {
            Triggers.Remove(trg);
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

        [RelayCommand]
        private void RemoveEffect(ParadoxEffect eff)
        {
            Effects.Remove(eff);
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

        public void Write(TextWriter writer)
        {
            if (Vanilla == false)
            {
                writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{Name} = {{");
                ParadoxText.IndentLevel++;
            }
            else
            {
                writer.Indent().WriteLine($"{Name} = {{");
                ParadoxText.IndentLevel++;
            }

            WriteTrigger(writer);

            writer.WriteLine();

            WriteEffect(writer);

            writer.WriteLine();

            WriteEvents(writer);

            writer.WriteLine();

            WriteOnActions(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
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

        private void WriteEffect(TextWriter writer)
        {
            if (Effect == string.Empty && Effects.Count == 0)
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

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteEvents(TextWriter writer)
        {
            if (Events.Count == 0)
            {
                writer.Indent().WriteLine("# no events");
                return;
            }

            writer.Indent().WriteLine("random_events = {");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"chance_to_happen = {Chance}");
            writer.WriteLine();

            foreach (ParadoxEvent evt in Events)
            {
                writer.Indent().WriteLine($"{evt.Weight} = {Context.Current.Info.EventNamespace}.{evt.Id}");
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteOnActions(TextWriter writer)
        {
            if (OnActions.Count == 0)
            {
                writer.Indent().WriteLine("# no on-actions");
                return;
            }

            writer.Indent().WriteLine("on_actions = {");
            ParadoxText.IndentLevel++;

            foreach (ParadoxOnAction act in OnActions)
            {
                if (act.Vanilla == false)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{act.Name}");
                    ParadoxText.IndentLevel++;
                }
                else
                {
                    writer.Indent().WriteLine($"{act.Name}");
                    ParadoxText.IndentLevel++;
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }
}

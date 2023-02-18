using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Paradoxical.Model
{
    public partial class ParadoxOnAction : ParadoxElement
    {
        public Context CurrentContext => Context.Current;

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

        public ParadoxOnAction() : base()
        {
            Context.Current.OnActions.Add(this);
        }

        public ParadoxOnAction(ParadoxOnAction other) : this()
        {
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

        public override void Delete()
        {
            foreach (ParadoxOnAction act in Context.Current.OnActions)
            {
                act.OnActions.Remove(this);
            }

            Context.Current.OnActions.Remove(this);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Events);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(Events);

            FindDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = new(elements),
                Blacklist = new(blacklist),
                ElementType = typeof(ParadoxEvent),
            };
            FindDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            if (vm.Selected is not ParadoxEvent selected)
            { return; }

            Events.Add(selected);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.OnActions);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(OnActions);

            FindDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = new(elements),
                Blacklist = new(blacklist),
                ElementType = typeof(ParadoxOnAction),
            };
            FindDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            if (vm.Selected is not ParadoxOnAction selected)
            { return; }

            OnActions.Add(selected);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Triggers);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(Triggers);

            FindDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = new(elements),
                Blacklist = new(blacklist),
                ElementType = typeof(ParadoxTrigger),
            };
            FindDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            if (vm.Selected is not ParadoxTrigger selected)
            { return; }

            Triggers.Add(selected);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Effects);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(Effects);

            FindDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = new(elements),
                Blacklist = new(blacklist),
                ElementType = typeof(ParadoxEffect),
            };
            FindDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            if (vm.Selected is not ParadoxEffect selected)
            { return; }

            Effects.Add(selected);
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
                }
                else
                {
                    writer.Indent().WriteLine($"{act.Name}");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }
}

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
    public partial class ParadoxEvent : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private int id = 0;

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private int weight = 100;
        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string description = "";
        [ObservableProperty]
        private string theme = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEventOption> options = new();

        [ObservableProperty]
        private ParadoxPortrait leftPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait rightPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerLeftPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerRightPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerCenterPortrait = new();

        [ObservableProperty]
        private string trigger = "";
        [ObservableProperty]
        private string triggerTooltip = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> triggers = new();

        [ObservableProperty]
        private string immediateEffect = "";
        [ObservableProperty]
        private string immediateTooltip = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> immediateEffects = new();

        [ObservableProperty]
        private string afterEffect = "";
        [ObservableProperty]
        private string afterTooltip = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> afterEffects = new();

        public ParadoxEvent()
        {
            id = Context.Current.Events.Count == 0 ? 1 : Context.Current.Events.Max(evt => evt.Id) + 1;
            name = $"Event_{Guid.NewGuid().ToString()[0..4]}";
            title = "Hello World";
            description = "Hello World!";
        }

        public ParadoxEvent(ParadoxEvent other) : this()
        {
            title = other.title;
            description = other.description;
            theme = other.theme;

            // composite association, therefore deep copy
            Options = new(other.Options.Select(e => new ParadoxEventOption(e)));

            // composite associations, therefore deep copy
            leftPortrait = new(other.leftPortrait);
            rightPortrait = new(other.rightPortrait);
            lowerLeftPortrait = new(other.lowerLeftPortrait);
            lowerRightPortrait = new(other.lowerRightPortrait);
            lowerCenterPortrait = new(other.lowerCenterPortrait);

            trigger = other.trigger;
            immediateEffect = other.immediateEffect;
            afterEffect = other.afterEffect;

            // aggregate association, therefore shallow copy
            Triggers = new(other.Triggers);
            ImmediateEffects = new(other.ImmediateEffects);
            AfterEffects = new(other.AfterEffects);
        }

        [RelayCommand]
        private void AddOption()
        {
            ParadoxEventOption opt = new();

            Options.Add(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void RemoveOption(ParadoxEventOption opt)
        {
            Options.Remove(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void DuplicateOption(ParadoxEventOption opt)
        {
            ParadoxEventOption copy = new(opt);
            Options.Add(copy);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanMoveOptionUp))]
        private void MoveOptionUp(ParadoxEventOption opt)
        {
            int index = Options.IndexOf(opt);
            Options.Move(index, index - 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionUp(ParadoxEventOption opt)
        {
            return Options.Any()
                && opt != Options.First();
        }

        [RelayCommand(CanExecute = nameof(CanMoveOptionDown))]
        private void MoveOptionDown(ParadoxEventOption opt)
        {
            int index = Options.IndexOf(opt);
            Options.Move(index, index + 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionDown(ParadoxEventOption opt)
        {
            return Options.Any()
                && opt != Options.Last();
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
        private void CreateImmediateEffect()
        {
            ParadoxEffect eff = new();

            Context.Current.Effects.Add(eff);
            ImmediateEffects.Add(eff);
        }

        [RelayCommand]
        private void RemoveImmediateEffect(ParadoxEffect eff)
        {
            ImmediateEffects.Remove(eff);
        }

        [RelayCommand]
        private async void FindImmediateEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Effects,
                Blacklist = new(ImmediateEffects),
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

            ImmediateEffects.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateAfterEffect()
        {
            ParadoxEffect eff = new();

            Context.Current.Effects.Add(eff);
            AfterEffects.Add(eff);
        }

        [RelayCommand]
        private void RemoveAfterEffect(ParadoxEffect eff)
        {
            AfterEffects.Remove(eff);
        }

        [RelayCommand]
        private async void FindAfterEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Effects,
                Blacklist = new(AfterEffects),
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

            AfterEffects.Add(vm.Selected);
        }

        public void Write(TextWriter writer)
        {
            writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}.{Id} = {{");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"type = character_event");

            writer.WriteLine();

            writer.Indent().WriteLine($"title = {Context.Current.Info.EventNamespace}.{Id}.t");
            writer.Indent().WriteLine($"desc = {Context.Current.Info.EventNamespace}.{Id}.d");

            writer.WriteLine();

            WriteTheme(writer);

            writer.WriteLine();

            WriteLeftPortrait(writer);
            WriteRightPortrait(writer);
            WriteLowerLeftPortrait(writer);
            WriteLowerCenterPortrait(writer);
            WriteLowerRightPortrait(writer);

            writer.WriteLine();

            WriteTrigger(writer);

            writer.WriteLine();

            WriteImmediate(writer);

            writer.WriteLine();

            WriteAfter(writer);

            writer.WriteLine();

            WriteOptions(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteTheme(TextWriter writer)
        {
            if (Theme == string.Empty)
            {
                writer.Indent().WriteLine("# no theme");
                return;
            }

            writer.Indent().WriteLine($"theme = {Theme}");
        }

        private void WriteLeftPortrait(TextWriter writer)
        {
            if (LeftPortrait.Character == string.Empty)
            {
                writer.Indent().WriteLine("# no left portrait");
                return;
            }

            writer.Indent().WriteLine("left_portrait = {");
            ParadoxText.IndentLevel++;

            LeftPortrait.Write(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteRightPortrait(TextWriter writer)
        {
            if (RightPortrait.Character == string.Empty)
            {
                writer.Indent().WriteLine("# no right portrait");
                return;
            }

            writer.Indent().WriteLine("right_portrait = {");
            ParadoxText.IndentLevel++;

            RightPortrait.Write(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteLowerLeftPortrait(TextWriter writer)
        {
            if (LowerLeftPortrait.Character == string.Empty)
            {
                writer.Indent().WriteLine("# no lower left portrait");
                return;
            }

            writer.Indent().WriteLine("lower_left_portrait = {");
            ParadoxText.IndentLevel++;

            LowerLeftPortrait.Write(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteLowerCenterPortrait(TextWriter writer)
        {
            if (LowerCenterPortrait.Character == string.Empty)
            {
                writer.Indent().WriteLine("# no lower center portrait");
                return;
            }

            writer.Indent().WriteLine("lower_center_portrait = {");
            ParadoxText.IndentLevel++;

            LowerCenterPortrait.Write(writer);

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteLowerRightPortrait(TextWriter writer)
        {
            if (LowerRightPortrait.Character == string.Empty)
            {
                writer.Indent().WriteLine("# no lower right portrait");
                return;
            }

            writer.Indent().WriteLine("lower_right_portrait = {");
            ParadoxText.IndentLevel++;

            LowerRightPortrait.Write(writer);

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
                if (TriggerTooltip != string.Empty)
                {
                    writer.Indent().WriteLine("custom_tooltip = {");
                    ParadoxText.IndentLevel++;

                    writer.Indent().WriteLine($"text = {Context.Current.Info.EventNamespace}.{Id}.trigger.tt");
                    writer.WriteLine();
                }

                foreach (string line in Trigger.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }

                if (TriggerTooltip != string.Empty)
                {
                    ParadoxText.IndentLevel--;
                    writer.Indent().WriteLine("}");
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

        private void WriteImmediate(TextWriter writer)
        {
            if (ImmediateEffect == string.Empty && ImmediateEffects.Count == 0)
            {
                writer.Indent().WriteLine("# no immediate");
                return;
            }

            writer.Indent().WriteLine("immediate = {");
            ParadoxText.IndentLevel++;

            if (ImmediateEffect == string.Empty)
            {
                writer.Indent().WriteLine("# no custom effect");
            }
            else
            {
                if (ImmediateTooltip != string.Empty)
                {
                    writer.Indent().WriteLine("custom_tooltip = {");
                    ParadoxText.IndentLevel++;

                    writer.Indent().WriteLine($"text = {Context.Current.Info.EventNamespace}.{Id}.immediate.tt");
                    writer.WriteLine();
                }

                foreach (string line in ImmediateEffect.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }

                if (ImmediateTooltip != string.Empty)
                {
                    ParadoxText.IndentLevel--;
                    writer.Indent().WriteLine("}");
                }
            }

            if (ImmediateEffects.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted effects");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted effects");

                foreach (ParadoxEffect eff in ImmediateEffects)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{eff.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteAfter(TextWriter writer)
        {
            if (AfterEffect == string.Empty && AfterEffects.Count == 0)
            {
                writer.Indent().WriteLine("# no after");
                return;
            }

            writer.Indent().WriteLine("after = {");
            ParadoxText.IndentLevel++;

            if (AfterEffect == string.Empty)
            {
                writer.Indent().WriteLine("# no custom effect");
            }
            else
            {
                if (AfterTooltip != string.Empty)
                {
                    writer.Indent().WriteLine("custom_tooltip = {");
                    ParadoxText.IndentLevel++;

                    writer.Indent().WriteLine($"text = {Context.Current.Info.EventNamespace}.{Id}.after.tt");
                    writer.WriteLine();
                }

                foreach (string line in AfterEffect.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
                }

                if (AfterTooltip != string.Empty)
                {
                    ParadoxText.IndentLevel--;
                    writer.Indent().WriteLine("}");
                }
            }

            if (AfterEffects.Count == 0)
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# no scripted effects");
            }
            else
            {
                writer.WriteLine();
                writer.Indent().WriteLine("# scripted effects");

                foreach (ParadoxEffect eff in AfterEffects)
                {
                    writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{eff.Name} = yes");
                }
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }

        private void WriteOptions(TextWriter writer)
        {
            if (Options.Count == 0)
            {
                writer.Indent().WriteLine("# no options");
                return;
            }

            foreach (ParadoxEventOption opt in Options)
            {
                opt.Write(writer, this, Options.IndexOf(opt));
            }
        }

        public void WriteLoc(TextWriter writer)
        {
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Id}.t", Title);
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Id}.d", Description);

            if (TriggerTooltip != string.Empty)
            {
                writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Id}.trigger.tt", TriggerTooltip);
            }
            if (ImmediateTooltip != string.Empty)
            {
                writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Id}.immediate.tt", ImmediateTooltip);
            }
            if (AfterTooltip != string.Empty)
            {
                writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Id}.after.tt", AfterTooltip);
            }

            foreach (ParadoxEventOption opt in Options)
            {
                opt.WriteLoc(writer, this, Options.IndexOf(opt));
            }
        }
    }
}

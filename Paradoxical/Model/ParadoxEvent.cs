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
    public partial class ParadoxEvent : ParadoxElement
    {
        private const string DEFAULT_THEME = "default";

        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private int id = 0;

        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string description = "";
        [ObservableProperty]
        private string theme = "";
        [ObservableProperty]
        private bool hidden = false;

        [ObservableProperty]
        private int weight = 100;
        [ObservableProperty]
        private int cooldown = 0;

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
        private ObservableCollection<ParadoxTrigger> triggers = new();

        [ObservableProperty]
        private string immediateEffect = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> immediateEffects = new();

        [ObservableProperty]
        private string afterEffect = "";
        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> afterEffects = new();

        public ParadoxEvent() : base()
        {
            Context.Current.Events.Add(this);

            id = Context.Current.Events.Count == 0 ? 1 : Context.Current.Events.Max(evt => evt.Id) + 1;

            title = "Hello World";
            description = "Hello World!";
        }

        public ParadoxEvent(ParadoxEvent other) : this()
        {
            title = other.title;
            description = other.description;
            theme = other.theme;
            hidden = other.hidden;

            weight = other.weight;
            cooldown = other.cooldown;

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

        public override void Delete()
        {
            foreach (ParadoxEvent evt in Context.Current.Events)
            {
                foreach (ParadoxEventOption opt in evt.Options)
                {
                    if (opt.TriggeredEvent == this)
                    {
                        opt.TriggeredEvent = null;
                    }
                }
            }

            foreach (ParadoxDecision dec in Context.Current.Decisions)
            {
                if (dec.TriggeredEvent == this)
                {
                    dec.TriggeredEvent = null;
                }
            }

            foreach (ParadoxOnAction act in Context.Current.OnActions)
            {
                act.Events.Remove(this);
            }

            Context.Current.Events.Remove(this);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Effects);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(ImmediateEffects);

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

            ImmediateEffects.Add(selected);
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
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Effects);

            var blacklist = Enumerable.Empty<ParadoxElement>()
                .Union(AfterEffects);

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

            AfterEffects.Add(selected);
        }

        public void Write(TextWriter writer)
        {
            writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}.{Id} = {{");
            ParadoxText.IndentLevel++;

            writer.Indent().WriteLine($"type = character_event");

            if (Hidden)
            {
                writer.Indent().WriteLine("hidden = yes");
            }

            if (Hidden == false)
            {
                writer.WriteLine();

                writer.Indent().WriteLine($"title = {Context.Current.Info.EventNamespace}.{Id}.t");
                writer.Indent().WriteLine($"desc = {Context.Current.Info.EventNamespace}.{Id}.d");
                writer.Indent().WriteLine($"theme = {(Theme == string.Empty ? DEFAULT_THEME : Theme)}");
            }

            writer.WriteLine();
            WriteCooldown(writer);

            if (Hidden == false)
            {
                writer.WriteLine();

                WriteLeftPortrait(writer);
                WriteRightPortrait(writer);
                WriteLowerLeftPortrait(writer);
                WriteLowerCenterPortrait(writer);
                WriteLowerRightPortrait(writer);
            }

            writer.WriteLine();
            WriteTrigger(writer);

            writer.WriteLine();
            WriteImmediate(writer);

            writer.WriteLine();
            WriteAfter(writer);

            if (Hidden == false)
            {
                writer.WriteLine();

                WriteOptions(writer);
            }

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
                foreach (string line in ImmediateEffect.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
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
                foreach (string line in AfterEffect.Split(Environment.NewLine))
                {
                    writer.Indent().WriteLine(line);
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

            foreach (ParadoxEventOption opt in Options)
            {
                opt.WriteLoc(writer, this, Options.IndexOf(opt));
            }
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paradoxical.Model
{
    public partial class ParadoxEvent : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private int id = 0;

        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string description = "";
        [ObservableProperty]
        private string theme = "";
        [ObservableProperty]
        private string background = "";
        [ObservableProperty]
        private string sound = "";

        public ObservableCollection<ParadoxEventOption> Options { get; } = new();

        [ObservableProperty]
        private ParadoxPortrait leftPortrait;
        [ObservableProperty]
        private ParadoxPortrait rightPortrait;
        [ObservableProperty]
        private ParadoxPortrait lowerLeftPortrait;
        [ObservableProperty]
        private ParadoxPortrait lowerRightPortrait;
        [ObservableProperty]
        private ParadoxPortrait lowerCenterPortrait;

        [ObservableProperty]
        private string trigger = "";
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();

        [ObservableProperty]
        private string immediateEffect = "";
        public ObservableCollection<ParadoxEffect> ImmediateEffects { get; } = new();

        [ObservableProperty]
        private string afterEffect = "";
        public ObservableCollection<ParadoxEffect> AfterEffects { get; } = new();

        public ParadoxEvent(ModContext context)
        {
            Context = context;

            id = context.Events.Count == 0 ? 1 : context.Events.Max(evt => evt.Id) + 1;
            title = $"Event [{Guid.NewGuid().ToString()[0..4]}]";

            leftPortrait = new(context);
            rightPortrait = new(context);
            lowerLeftPortrait = new(context);
            lowerCenterPortrait = new(context);
            lowerRightPortrait = new(context);
        }

        public ParadoxEvent(ModContext context, ParadoxEvent other) : this(context)
        {
            title = other.title;
            description = other.description;

            theme = other.theme;
            background = other.background;
            sound = other.sound;

            // composite association, therefore deep copy
            Options = new(other.Options.Select(e => new ParadoxEventOption(context, e)));

            // composite associations, therefore deep copy
            leftPortrait = new(context, other.leftPortrait);
            rightPortrait = new(context, other.rightPortrait);
            lowerLeftPortrait = new(context, other.lowerLeftPortrait);
            lowerRightPortrait = new(context, other.lowerRightPortrait);
            lowerCenterPortrait = new(context, other.lowerCenterPortrait);

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
            ParadoxEventOption opt = new(Context);

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
            ParadoxEventOption copy = new(Context, opt);
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
            ParadoxTrigger trg = new(Context);

            Context.Triggers.Add(trg);
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
        private void CreateImmediateEffect()
        {
            ParadoxEffect eff = new(Context);

            Context.Effects.Add(eff);
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
                Items = Context.Effects,
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
            ParadoxEffect eff = new(Context);

            Context.Effects.Add(eff);
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
                Items = Context.Effects,
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
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Effects;

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

            leftPortrait = new(context);
            rightPortrait = new(context);
            lowerLeftPortrait = new(context);
            lowerCenterPortrait = new(context);
            lowerRightPortrait = new(context);
        }

        [RelayCommand]
        private void AddOption()
        {
            ParadoxEventOption opt = new(Context)
            {
                Name = "New Option"
            };

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
            ParadoxEventOption copy = new(Context)
            {
                Name = opt.Name,
                Tooltip = opt.Tooltip,
                TriggeredEvent = opt.TriggeredEvent,
                TriggeredEventScope = opt.TriggeredEventScope,
                Trigger = opt.Trigger,
                Effect = opt.Effect,
                AiChance = opt.AiChance,
                AiBaseChance = opt.AiBaseChance,
                AiBoldnessTargetModifier = opt.AiBoldnessTargetModifier,
                AiGreedTargetModifier = opt.AiGreedTargetModifier,
                AiCompassionTargetModifier = opt.AiCompassionTargetModifier,
                AiEnergyTargetModifier = opt.AiEnergyTargetModifier,
                AiHonorTargetModifier = opt.AiHonorTargetModifier,
                AiRationalityTargetModifier = opt.AiRationalityTargetModifier,
                AiSociabilityTargetModifier = opt.AiSociabilityTargetModifier,
                AiVengefulnessTargetModifier = opt.AiVengefulnessTargetModifier,
                AiZealTargetModifier = opt.AiZealTargetModifier,
            };

            foreach (var trigger in opt.Triggers)
            {
                copy.Triggers.Add(trigger);
            }

            foreach (var effect in opt.Effects)
            {
                copy.Effects.Add(effect);
            }

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
        private async void AddTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                Items = Context.Triggers,
                Blacklist = new(Triggers),
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
        private void RemoveTrigger(ParadoxTrigger trg)
        {
            Triggers.Remove(trg);
        }

        [RelayCommand]
        private async void AddImmediateEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                Items = Context.Effects,
                Blacklist = new(ImmediateEffects),
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            ImmediateEffects.Add(vm.Selected);
        }

        [RelayCommand]
        private void RemoveImmediateEffect(ParadoxEffect eff)
        {
            ImmediateEffects.Remove(eff);
        }

        [RelayCommand]
        private async void AddAfterEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                Items = Context.Effects,
                Blacklist = new(AfterEffects),
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            AfterEffects.Add(vm.Selected);
        }

        [RelayCommand]
        private void RemoveAfterEffect(ParadoxEffect eff)
        {
            AfterEffects.Remove(eff);
        }
    }
}

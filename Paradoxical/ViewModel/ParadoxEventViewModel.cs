using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxEventViewModel : ObservableObject
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

        public ObservableCollection<ParadoxEventOptionViewModel> Options { get; } = new();

        [ObservableProperty]
        private ParadoxPortraitViewModel leftPortrait;
        [ObservableProperty]
        private ParadoxPortraitViewModel rightPortrait;
        [ObservableProperty]
        private ParadoxPortraitViewModel lowerLeftPortrait;
        [ObservableProperty]
        private ParadoxPortraitViewModel lowerRightPortrait;
        [ObservableProperty]
        private ParadoxPortraitViewModel lowerCenterPortrait;

        [ObservableProperty]
        private string trigger = "";
        public ObservableCollection<ParadoxTriggerViewModel> Triggers { get; } = new();

        [ObservableProperty]
        private string immediateEffect = "";
        public ObservableCollection<ParadoxEffectViewModel> ImmediateEffects { get; } = new();

        [ObservableProperty]
        private string afterEffect = "";
        public ObservableCollection<ParadoxEffectViewModel> AfterEffects { get; } = new();

        public ParadoxEventViewModel(ModContext context)
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
            ParadoxEventOptionViewModel opt = new(Context)
            {
                Name = "New Option"
            };

            Options.Add(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void RemoveOption(ParadoxEventOptionViewModel opt)
        {
            Options.Remove(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void DuplicateOption(ParadoxEventOptionViewModel opt)
        {
            ParadoxEventOptionViewModel copy = new(Context)
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
        private void MoveOptionUp(ParadoxEventOptionViewModel opt)
        {
            int index = Options.IndexOf(opt);
            Options.Move(index, index - 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionUp(ParadoxEventOptionViewModel opt)
        {
            return Options.Any()
                && opt != Options.First();
        }

        [RelayCommand(CanExecute = nameof(CanMoveOptionDown))]
        private void MoveOptionDown(ParadoxEventOptionViewModel opt)
        {
            int index = Options.IndexOf(opt);
            Options.Move(index, index + 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionDown(ParadoxEventOptionViewModel opt)
        {
            return Options.Any()
                && opt != Options.Last();
        }
    }
}

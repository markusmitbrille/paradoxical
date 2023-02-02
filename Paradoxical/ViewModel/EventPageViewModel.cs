using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;

namespace Paradoxical.ViewModel
{
    public partial class EventPageViewModel : PageViewModel
    {
        public override string PageName => "Events";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveOptionUpCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveOptionDownCommand))]
        [NotifyCanExecuteChangedFor(nameof(FindEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEventSelected))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateOptionCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveOptionUpCommand))]
        [NotifyCanExecuteChangedFor(nameof(MoveOptionDownCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEvent? selectedEvent;
        public bool IsEventSelected => SelectedEvent != null;

        [RelayCommand(CanExecute = nameof(CanAddEvent))]
        private void AddEvent()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxEvent evt = new()
            {
                Id = ActiveMod.Events.Count == 0 ? 1 : ActiveMod.Events.Max(evt => evt.Id) + 1,
                Title = "New Event"
            };

            ActiveMod.Events.Add(evt);
            SelectedEvent = evt;
        }
        private bool CanAddEvent()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEvent))]
        private void RemoveEvent()
        {
            if (ActiveMod == null)
            { return; }
            if (SelectedEvent == null)
            { return; }

            foreach (ParadoxEvent evt in ActiveMod.Events)
            {
                foreach (ParadoxEventOption opt in evt.Options)
                {
                    if (opt.TriggeredEvent == SelectedEvent)
                    {
                        opt.TriggeredEvent = null;
                    }
                }
            }

            ActiveMod.Events.Remove(SelectedEvent);
            SelectedEvent = ActiveMod.Events.FirstOrDefault();
        }
        private bool CanRemoveEvent()
        {
            return ActiveMod != null && SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindEvent))]
        private async void FindEvent()
        {
            if (ActiveMod == null)
            { return; }

            FindEventDialogViewModel vm = new()
            {
                Items = ActiveMod.Events,
            };
            FindEventDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            SelectedEvent = vm.Selected;
        }
        private bool CanFindEvent()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanPreviousEvent))]
        private void PreviousEvent()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            int index = ActiveMod.Events.IndexOf(SelectedEvent);
            SelectedEvent = ActiveMod.Events[index - 1];
        }
        private bool CanPreviousEvent()
        {
            return ActiveMod != null
                && SelectedEvent != null
                && ActiveMod.Events.Any()
                && SelectedEvent != ActiveMod.Events.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextEvent))]
        private void NextEvent()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            int index = ActiveMod.Events.IndexOf(SelectedEvent);
            SelectedEvent = ActiveMod.Events[index + 1];
        }
        private bool CanNextEvent()
        {
            return ActiveMod != null
                && SelectedEvent != null
                && ActiveMod.Events.Any()
                && SelectedEvent != ActiveMod.Events.Last();
        }

        [RelayCommand(CanExecute = nameof(CanAddOption))]
        private void AddOption()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            ParadoxEventOption opt = new()
            {
                Name = "New Option"
            };

            SelectedEvent.Options.Add(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanAddOption()
        {
            return ActiveMod != null && SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveOption))]
        private void RemoveOption(ParadoxEventOption opt)
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            SelectedEvent.Options.Remove(opt);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveOption(ParadoxEventOption opt)
        {
            return ActiveMod != null && SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateOption))]
        private void DuplicateOption(ParadoxEventOption opt)
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            ParadoxEventOption copy = new()
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

            SelectedEvent.Options.Add(copy);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanDuplicateOption(ParadoxEventOption opt)
        {
            return ActiveMod != null && SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanMoveOptionUp))]
        private void MoveOptionUp(ParadoxEventOption opt)
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            int index = SelectedEvent.Options.IndexOf(opt);
            SelectedEvent.Options.Move(index, index - 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionUp(ParadoxEventOption opt)
        {
            return ActiveMod != null
                && SelectedEvent != null
                && SelectedEvent.Options.Any()
                && opt != SelectedEvent.Options.First();
        }

        [RelayCommand(CanExecute = nameof(CanMoveOptionDown))]
        private void MoveOptionDown(ParadoxEventOption opt)
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            int index = SelectedEvent.Options.IndexOf(opt);
            SelectedEvent.Options.Move(index, index + 1);

            MoveOptionUpCommand.NotifyCanExecuteChanged();
            MoveOptionDownCommand.NotifyCanExecuteChanged();
        }
        private bool CanMoveOptionDown(ParadoxEventOption opt)
        {
            return ActiveMod != null
                && SelectedEvent != null
                && SelectedEvent.Options.Any()
                && opt != SelectedEvent.Options.Last();
        }
    }
}
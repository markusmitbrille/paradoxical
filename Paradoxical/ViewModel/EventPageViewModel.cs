using CommunityToolkit.Mvvm.ComponentModel;
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
        [NotifyCanExecuteChangedFor(nameof(FindEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxModViewModel? activeMod;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEventSelected))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEventViewModel? selectedEvent;
        public bool IsEventSelected => SelectedEvent != null;

        [RelayCommand(CanExecute = nameof(CanAddEvent))]
        private void AddEvent()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxEventViewModel evt = new()
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

            foreach (ParadoxEventViewModel evt in ActiveMod.Events)
            {
                foreach (ParadoxEventOptionViewModel opt in evt.Options)
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
    }
}
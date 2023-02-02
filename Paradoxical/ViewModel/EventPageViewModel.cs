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
        [NotifyCanExecuteChangedFor(nameof(FindEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEvent? selectedEvent;

        [RelayCommand(CanExecute = nameof(CanAddEvent))]
        private void AddEvent()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxEvent evt = new();
            evt.Id = ActiveMod.Events.Count == 0 ? 1 : ActiveMod.Events.Max(evt => evt.Id) + 1;
            evt.Title = "New Event";

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
                    if (opt.TriggerEvent == SelectedEvent)
                    {
                        opt.TriggerEvent = null;
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

            for (int i = SelectedEvent.Id - 1; i > 0; i--)
            {
                ParadoxEvent? evt = ActiveMod.Events.SingleOrDefault(evt => evt.Id == i);
                if (evt != null)
                {
                    SelectedEvent = evt;
                    return;
                }
            }
        }
        private bool CanPreviousEvent()
        {
            if (ActiveMod == null)
            { return false; }

            if (SelectedEvent == null)
            { return false; }

            for (int i = SelectedEvent.Id - 1; i > 0; i--)
            {
                if (ActiveMod.Events.Any(evt => evt.Id == i))
                { return true; }
            }

            return false;
        }

        [RelayCommand(CanExecute = nameof(CanNextEvent))]
        private void NextEvent()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedEvent == null)
            { return; }

            int maxId = ActiveMod.Events.Max(evt => evt.Id);
            for (int i = SelectedEvent.Id + 1; i <= maxId; i++)
            {
                ParadoxEvent? evt = ActiveMod.Events.SingleOrDefault(evt => evt.Id == i);
                if (evt != null)
                {
                    SelectedEvent = evt;
                    return;
                }
            }
        }
        private bool CanNextEvent()
        {
            if (ActiveMod == null)
            { return false; }

            if (SelectedEvent == null)
            { return false; }

            int maxId = ActiveMod.Events.Max(evt => evt.Id);
            for (int i = SelectedEvent.Id + 1; i <= maxId; i++)
            {
                if (ActiveMod.Events.Any(evt => evt.Id == i))
                { return true; }
            }

            return false;
        }
    }
}
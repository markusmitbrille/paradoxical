using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Data;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class EventPageViewModel : PageViewModel
    {
        public override string PageName => "Events";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEventSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEvent? selectedEvent;
        public bool IsEventSelected => SelectedEvent != null;

        public EventPageViewModel(ModContext context)
        {
            Context = context;
        }

        [RelayCommand]
        private void AddEvent()
        {
            ParadoxEvent evt = new(Context);

            Context.Events.Add(evt);
            SelectedEvent = evt;
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateEvent))]
        private void DuplicateEvent()
        {
            if (SelectedEvent == null)
            { return; }

            ParadoxEvent evt = new(Context, SelectedEvent);
            Context.Events.Add(evt);

            SelectedEvent = evt;
        }
        private bool CanDuplicateEvent()
        {
            return SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEvent))]
        private void RemoveEvent()
        {
            if (SelectedEvent == null)
            { return; }

            if (MessageBox.Show(
                "Are you sure?",
                "Remove Event",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            foreach (ParadoxEvent evt in Context.Events)
            {
                foreach (ParadoxEventOption opt in evt.Options)
                {
                    if (opt.TriggeredEvent == SelectedEvent)
                    {
                        opt.TriggeredEvent = null;
                    }
                }
            }

            Context.Events.Remove(SelectedEvent);
            SelectedEvent = Context.Events.FirstOrDefault();
        }
        private bool CanRemoveEvent()
        {
            return SelectedEvent != null;
        }

        [RelayCommand]
        private async void FindEvent()
        {
            FindEventDialogViewModel vm = new()
            {
                Items = Context.Events,
                Selected = SelectedEvent,
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

        [RelayCommand(CanExecute = nameof(CanPreviousEvent))]
        private void PreviousEvent()
        {
            if (SelectedEvent == null)
            { return; }

            int index = Context.Events.IndexOf(SelectedEvent);
            SelectedEvent = Context.Events[index - 1];
        }
        private bool CanPreviousEvent()
        {
            return SelectedEvent != null
                && Context.Events.Any()
                && SelectedEvent != Context.Events.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextEvent))]
        private void NextEvent()
        {
            if (SelectedEvent == null)
            { return; }

            int index = Context.Events.IndexOf(SelectedEvent);
            SelectedEvent = Context.Events[index + 1];
        }
        private bool CanNextEvent()
        {
            return SelectedEvent != null
                && Context.Events.Any()
                && SelectedEvent != Context.Events.Last();
        }
    }
}
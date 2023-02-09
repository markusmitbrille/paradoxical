using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class EventPageViewModel : PageViewModelBase
    {
        public override string PageName => "Events";

        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEventSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEvent? selectedEvent;
        public bool IsEventSelected => SelectedEvent != null;

        public EventPageViewModel()
        {
        }

        [RelayCommand]
        private void AddEvent()
        {
            ParadoxEvent evt = new();

            Context.Current.Events.Add(evt);
            SelectedEvent = evt;

            FindEventCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateEvent))]
        private void DuplicateEvent()
        {
            if (SelectedEvent == null)
            { return; }

            ParadoxEvent evt = new(SelectedEvent);

            Context.Current.Events.Add(evt);
            SelectedEvent = evt;

            FindEventCommand.NotifyCanExecuteChanged();
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

            foreach (ParadoxEvent evt in Context.Current.Events)
            {
                foreach (ParadoxEventOption opt in evt.Options)
                {
                    if (opt.TriggeredEvent == SelectedEvent)
                    {
                        opt.TriggeredEvent = null;
                    }
                }
            }

            Context.Current.Events.Remove(SelectedEvent);
            SelectedEvent = Context.Current.Events.FirstOrDefault();

            FindEventCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveEvent()
        {
            return SelectedEvent != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindEvent))]
        private async void FindEvent()
        {
            FindEventDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Events,
                Selected = SelectedEvent,
            };
            FindEventDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            SelectedEvent = vm.Selected;
        }
        private bool CanFindEvent()
        {
            return Context.Current.Events.Any();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousEvent))]
        private void PreviousEvent()
        {
            if (SelectedEvent == null)
            { return; }

            int index = Context.Current.Events.IndexOf(SelectedEvent);
            SelectedEvent = Context.Current.Events[index - 1];
        }
        private bool CanPreviousEvent()
        {
            return SelectedEvent != null
                && Context.Current.Events.Any()
                && SelectedEvent != Context.Current.Events.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextEvent))]
        private void NextEvent()
        {
            if (SelectedEvent == null)
            { return; }

            int index = Context.Current.Events.IndexOf(SelectedEvent);
            SelectedEvent = Context.Current.Events[index + 1];
        }
        private bool CanNextEvent()
        {
            return SelectedEvent != null
                && Context.Current.Events.Any()
                && SelectedEvent != Context.Current.Events.Last();
        }
    }
}
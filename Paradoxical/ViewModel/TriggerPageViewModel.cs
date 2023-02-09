using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class TriggerPageViewModel : PageViewModelBase
    {
        public override string PageName => "Triggers";

        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTriggerSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextTriggerCommand))]
        private ParadoxTrigger? selectedTrigger;
        public bool IsTriggerSelected => SelectedTrigger != null;

        public TriggerPageViewModel()
        {
            selectedTrigger = CurrentContext.Triggers.FirstOrDefault();
        }

        [RelayCommand]
        private void AddTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            SelectedTrigger = trg;

            FindTriggerCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateTrigger))]
        private void DuplicateTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            ParadoxTrigger evt = new(SelectedTrigger);

            Context.Current.Triggers.Add(evt);
            SelectedTrigger = evt;

            FindTriggerCommand.NotifyCanExecuteChanged();
        }
        private bool CanDuplicateTrigger()
        {
            return SelectedTrigger != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveTrigger))]
        private void RemoveTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            if (MessageBox.Show(
                "Are you sure?",
                "Remove Trigger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            foreach (ParadoxEvent evt in Context.Current.Events)
            {
                evt.Triggers.Remove(SelectedTrigger);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Triggers.Remove(SelectedTrigger);
                }
            }

            Context.Current.Triggers.Remove(SelectedTrigger);
            SelectedTrigger = Context.Current.Triggers.FirstOrDefault();

            FindTriggerCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveTrigger()
        {
            return SelectedTrigger != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindTrigger))]
        private async void FindTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Triggers,
                Selected = SelectedTrigger,
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

            SelectedTrigger = vm.Selected;
        }
        private bool CanFindTrigger()
        {
            return Context.Current.Triggers.Any();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousTrigger))]
        private void PreviousTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            int index = Context.Current.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = Context.Current.Triggers[index - 1];
        }
        private bool CanPreviousTrigger()
        {
            return SelectedTrigger != null
                && Context.Current.Triggers.Any()
                && SelectedTrigger != Context.Current.Triggers.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextTrigger))]
        private void NextTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            int index = Context.Current.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = Context.Current.Triggers[index + 1];
        }
        private bool CanNextTrigger()
        {
            return SelectedTrigger != null
                && Context.Current.Triggers.Any()
                && SelectedTrigger != Context.Current.Triggers.Last();
        }
    }
}
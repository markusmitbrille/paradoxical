using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class DecisionPageViewModel : PageViewModelBase
    {
        public override string PageName => "Decisions";

        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDecisionSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateDecisionCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveDecisionCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousDecisionCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextDecisionCommand))]
        private ParadoxDecision? selectedDecision;
        public bool IsDecisionSelected => SelectedDecision != null;

        public DecisionPageViewModel()
        {
            selectedDecision = CurrentContext.Decisions.FirstOrDefault();
        }

        [RelayCommand]
        private void AddDecision()
        {
            ParadoxDecision dec = new();

            Context.Current.Decisions.Add(dec);
            SelectedDecision = dec;

            FindDecisionCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateDecision))]
        private void DuplicateDecision()
        {
            if (SelectedDecision == null)
            { return; }

            ParadoxDecision evt = new(SelectedDecision);

            Context.Current.Decisions.Add(evt);
            SelectedDecision = evt;

            FindDecisionCommand.NotifyCanExecuteChanged();
        }
        private bool CanDuplicateDecision()
        {
            return SelectedDecision != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveDecision))]
        private void RemoveDecision()
        {
            if (SelectedDecision == null)
            { return; }

            if (MessageBox.Show(
                "Are you sure?",
                "Remove Decision",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            Context.Current.Decisions.Remove(SelectedDecision);
            SelectedDecision = Context.Current.Decisions.FirstOrDefault();

            FindDecisionCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveDecision()
        {
            return SelectedDecision != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindDecision))]
        private async void FindDecision()
        {
            FindDecisionDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Decisions,
                Selected = SelectedDecision,
            };
            FindDecisionDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            SelectedDecision = vm.Selected;
        }
        private bool CanFindDecision()
        {
            return Context.Current.Decisions.Any();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousDecision))]
        private void PreviousDecision()
        {
            if (SelectedDecision == null)
            { return; }

            int index = Context.Current.Decisions.IndexOf(SelectedDecision);
            SelectedDecision = Context.Current.Decisions[index - 1];
        }
        private bool CanPreviousDecision()
        {
            return SelectedDecision != null
                && Context.Current.Decisions.Any()
                && SelectedDecision != Context.Current.Decisions.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextDecision))]
        private void NextDecision()
        {
            if (SelectedDecision == null)
            { return; }

            int index = Context.Current.Decisions.IndexOf(SelectedDecision);
            SelectedDecision = Context.Current.Decisions[index + 1];
        }
        private bool CanNextDecision()
        {
            return SelectedDecision != null
                && Context.Current.Decisions.Any()
                && SelectedDecision != Context.Current.Decisions.Last();
        }
    }
}
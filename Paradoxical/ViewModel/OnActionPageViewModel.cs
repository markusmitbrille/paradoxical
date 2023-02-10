using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Model;
using Paradoxical.View;
using System.Linq;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class OnActionPageViewModel : PageViewModelBase
    {
        public override string PageName => "On-Actions";

        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOnActionSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateOnActionCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveOnActionCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousOnActionCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextOnActionCommand))]
        private ParadoxOnAction? selectedOnAction;
        public bool IsOnActionSelected => SelectedOnAction != null;

        public OnActionPageViewModel()
        {
            selectedOnAction = CurrentContext.OnActions.FirstOrDefault();
        }

        [RelayCommand]
        private void AddOnAction()
        {
            ParadoxOnAction act = new();

            Context.Current.OnActions.Add(act);
            SelectedOnAction = act;

            FindOnActionCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateOnAction))]
        private void DuplicateOnAction()
        {
            if (SelectedOnAction == null)
            { return; }

            ParadoxOnAction evt = new(SelectedOnAction);

            Context.Current.OnActions.Add(evt);
            SelectedOnAction = evt;

            FindOnActionCommand.NotifyCanExecuteChanged();
        }
        private bool CanDuplicateOnAction()
        {
            return SelectedOnAction != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveOnAction))]
        private void RemoveOnAction()
        {
            if (SelectedOnAction == null)
            { return; }

            if (MessageBox.Show(
                "Are you sure?",
                "Remove Trigger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            Context.Current.OnActions.Remove(SelectedOnAction);
            SelectedOnAction = Context.Current.OnActions.FirstOrDefault();

            FindOnActionCommand.NotifyCanExecuteChanged();
        }
        private bool CanRemoveOnAction()
        {
            return SelectedOnAction != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindOnAction))]
        private async void FindOnAction()
        {
            FindOnActionDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.OnActions,
                Selected = SelectedOnAction,
            };
            FindOnActionDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            SelectedOnAction = vm.Selected;
        }
        private bool CanFindOnAction()
        {
            return Context.Current.OnActions.Any();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousOnAction))]
        private void PreviousOnAction()
        {
            if (SelectedOnAction == null)
            { return; }

            int index = Context.Current.OnActions.IndexOf(SelectedOnAction);
            SelectedOnAction = Context.Current.OnActions[index - 1];
        }
        private bool CanPreviousOnAction()
        {
            return SelectedOnAction != null
                && Context.Current.OnActions.Any()
                && SelectedOnAction != Context.Current.OnActions.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextOnAction))]
        private void NextOnAction()
        {
            if (SelectedOnAction == null)
            { return; }

            int index = Context.Current.OnActions.IndexOf(SelectedOnAction);
            SelectedOnAction = Context.Current.OnActions[index + 1];
        }
        private bool CanNextOnAction()
        {
            return SelectedOnAction != null
                && Context.Current.OnActions.Any()
                && SelectedOnAction != Context.Current.OnActions.Last();
        }
    }
}
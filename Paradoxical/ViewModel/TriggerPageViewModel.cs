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
    public partial class TriggerPageViewModel : PageViewModel
    {
        public override string PageName => "Triggers";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTriggerSelected))]
        [NotifyCanExecuteChangedFor(nameof(DuplicateTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextTriggerCommand))]
        private ParadoxTrigger? selectedTrigger;
        public bool IsTriggerSelected => SelectedTrigger != null;

        public TriggerPageViewModel(ModContext context)
        {
            Context = context;
        }

        [RelayCommand]
        private void AddTrigger()
        {
            ParadoxTrigger trg = new(Context);

            Context.Triggers.Add(trg);
            SelectedTrigger = trg;
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateTrigger))]
        private void DuplicateTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            ParadoxTrigger evt = new(Context, SelectedTrigger);
            Context.Triggers.Add(evt);

            SelectedTrigger = evt;
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

            foreach (ParadoxEvent evt in Context.Events)
            {
                evt.Triggers.Remove(SelectedTrigger);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Triggers.Remove(SelectedTrigger);
                }
            }

            Context.Triggers.Remove(SelectedTrigger);
            SelectedTrigger = Context.Triggers.FirstOrDefault();
        }
        private bool CanRemoveTrigger()
        {
            return SelectedTrigger != null;
        }

        [RelayCommand]
        private async void FindTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Triggers,
                Selected = SelectedTrigger,
            };
            FindTriggerDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            SelectedTrigger = vm.Selected;
        }

        [RelayCommand(CanExecute = nameof(CanPreviousTrigger))]
        private void PreviousTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            int index = Context.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = Context.Triggers[index - 1];
        }
        private bool CanPreviousTrigger()
        {
            return SelectedTrigger != null
                && Context.Triggers.Any()
                && SelectedTrigger != Context.Triggers.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextTrigger))]
        private void NextTrigger()
        {
            if (SelectedTrigger == null)
            { return; }

            int index = Context.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = Context.Triggers[index + 1];
        }
        private bool CanNextTrigger()
        {
            return SelectedTrigger != null
                && Context.Triggers.Any()
                && SelectedTrigger != Context.Triggers.Last();
        }
    }
}
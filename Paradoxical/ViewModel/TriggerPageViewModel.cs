using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;

namespace Paradoxical.ViewModel
{
    public partial class TriggerPageViewModel : PageViewModel
    {
        public override string PageName => "Triggers";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(FindTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextTriggerCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTriggerSelected))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextTriggerCommand))]
        private ParadoxTrigger? selectedTrigger;
        public bool IsTriggerSelected => SelectedTrigger != null;

        [RelayCommand(CanExecute = nameof(CanAddTrigger))]
        private void AddTrigger()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxTrigger trg = new();
            trg.Name = "New Trigger";

            ActiveMod.Triggers.Add(trg);
            SelectedTrigger = trg;
        }
        private bool CanAddTrigger()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveTrigger))]
        private void RemoveTrigger()
        {
            if (ActiveMod == null)
            { return; }
            if (SelectedTrigger == null)
            { return; }

            foreach (ParadoxEvent evt in ActiveMod.Events)
            {
                evt.Triggers.Remove(SelectedTrigger);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Triggers.Remove(SelectedTrigger);
                }
            }

            ActiveMod.Triggers.Remove(SelectedTrigger);
            SelectedTrigger = ActiveMod.Triggers.FirstOrDefault();
        }
        private bool CanRemoveTrigger()
        {
            return ActiveMod != null && SelectedTrigger != null;
        }

        [RelayCommand(CanExecute = nameof(CanFindTrigger))]
        private async void FindTrigger()
        {
            if (ActiveMod == null)
            { return; }

            FindTriggerDialogViewModel vm = new()
            {
                Items = ActiveMod.Triggers,
            };
            FindTriggerDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, "RootDialog");

            if (vm.Selected == null)
            { return; }

            SelectedTrigger = vm.Selected;
        }
        private bool CanFindTrigger()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanPreviousTrigger))]
        private void PreviousTrigger()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedTrigger == null)
            { return; }

            int index = ActiveMod.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = ActiveMod.Triggers[index - 1];
        }
        private bool CanPreviousTrigger()
        {
            return ActiveMod != null
                && SelectedTrigger != null
                && ActiveMod.Triggers.Any()
                && SelectedTrigger != ActiveMod.Triggers.First();
        }

        [RelayCommand(CanExecute = nameof(CanNextTrigger))]
        private void NextTrigger()
        {
            if (ActiveMod == null)
            { return; }

            if (SelectedTrigger == null)
            { return; }

            int index = ActiveMod.Triggers.IndexOf(SelectedTrigger);
            SelectedTrigger = ActiveMod.Triggers[index + 1];
        }
        private bool CanNextTrigger()
        {
            return ActiveMod != null
                && SelectedTrigger != null
                && ActiveMod.Triggers.Any()
                && SelectedTrigger != ActiveMod.Triggers.Last();
        }
    }
}
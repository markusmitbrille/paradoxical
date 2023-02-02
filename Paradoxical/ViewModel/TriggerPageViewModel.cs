using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Paradoxical.ViewModel
{
    public partial class TriggerPageViewModel : PageViewModel
    {
        public override string PageName => "Triggers";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        private ParadoxTrigger? selectedTrigger;

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
    }
}
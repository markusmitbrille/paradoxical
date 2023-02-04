using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;
using Paradoxical.Data;
using Paradoxical.Model;
using System;

namespace Paradoxical.ViewModel
{
    public partial class TriggerPageViewModel : PageViewModel
    {
        public override string PageName => "Triggers";

        public ModContext Context { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTriggerSelected))]
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
            ParadoxTrigger trg = new(Context)
            {
                Name = $"Trigger [{Guid.NewGuid().ToString()[0..4]}]",
            };

            Context.Triggers.Add(trg);
            SelectedTrigger = trg;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveTrigger))]
        private void RemoveTrigger()
        {
            if (SelectedTrigger == null)
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
                Items = Context.Triggers,
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
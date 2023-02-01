using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using Paradoxical.View;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveModCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        [NotifyCanExecuteChangedFor(nameof(AddEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        [NotifyCanExecuteChangedFor(nameof(FindEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(PreviousEventCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextEventCommand))]
        private ParadoxEvent? selectedEvent;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveTriggerCommand))]
        private ParadoxTrigger? selectedTrigger;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveEffectCommand))]
        private ParadoxEffect? selectedEffect;

        public MainViewModel()
        {
        }

        [RelayCommand]
        private void NewMod()
        {
            if (ActiveMod != null && MessageBox.Show(
                "Do you want to continue? Unsaved changes will be discarded!",
                "New Mod",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }

            ActiveMod = new();
        }

        [RelayCommand]
        private void OpenMod()
        {
            if (ActiveMod != null && MessageBox.Show(
                "Do you want to continue? Unsaved changes will be discarded!",
                "New Mod",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }

            throw new NotImplementedException();
        }

        [RelayCommand(CanExecute = nameof(CanSaveMod))]
        private void SaveMod()
        {
            throw new NotImplementedException();
        }
        private bool CanSaveMod()
        {
            return ActiveMod != null;
        }

        [RelayCommand]
        private void ExportMod()
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        private void Exit()
        {
            if (ActiveMod != null && MessageBox.Show(
                "Do you want to continue? Unsaved changes will be discarded!",
                "New Mod",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }

            Application.Current.Shutdown();
        }

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

        [RelayCommand(CanExecute = nameof(CanAddEffect))]
        private void AddEffect()
        {
            if (ActiveMod == null)
            { return; }

            ParadoxEffect eff = new();
            eff.Name = "New Effect";

            ActiveMod.Effects.Add(eff);
            SelectedEffect = eff;
        }
        private bool CanAddEffect()
        {
            return ActiveMod != null;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveEffect))]
        private void RemoveEffect()
        {
            if (ActiveMod == null)
            { return; }
            if (SelectedEffect == null)
            { return; }

            foreach (ParadoxEvent evt in ActiveMod.Events)
            {
                evt.ImmediateEffects.Remove(SelectedEffect);
                evt.AfterEffects.Remove(SelectedEffect);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Effects.Remove(SelectedEffect);
                }
            }

            ActiveMod.Effects.Remove(SelectedEffect);
            SelectedEffect = ActiveMod.Effects.FirstOrDefault();
        }
        private bool CanRemoveEffect()
        {
            return ActiveMod != null && SelectedEffect != null;
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
            FindEventDialog dlg = new()
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
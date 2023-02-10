using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.View;
using Paradoxical.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace Paradoxical.Model
{
    public partial class ParadoxOnAction : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent> events = new();

        [ObservableProperty]
        private ObservableCollection<ParadoxOnAction> onActions = new();

        [ObservableProperty]
        private string trigger = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> triggers = new();

        [ObservableProperty]
        private string effect = "";

        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> effects = new();

        public ParadoxOnAction()
        {
            name = $"OnAction_{Guid.NewGuid().ToString()[0..4]}";
        }

        public ParadoxOnAction(ParadoxOnAction other) : this()
        {
            name = other.name;

            // aggregate association, therefore shallow copy
            Events = new(other.Events);
            OnActions = new(other.OnActions);

            trigger = other.trigger;
            effect = other.effect;

            // aggregate association, therefore shallow copy
            Triggers = new(other.Triggers);
            Effects = new(other.Effects);
        }

        [RelayCommand]
        private void CreateEvent()
        {
            ParadoxEvent trg = new();

            Context.Current.Events.Add(trg);
            Events.Add(trg);
        }

        [RelayCommand]
        private void RemoveEvent(ParadoxEvent evt)
        {
            Events.Remove(evt);
        }

        [RelayCommand]
        private async void FindEvent()
        {
            FindEventDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Events,
                Blacklist = new(Events),
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

            Events.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateOnAction()
        {
            ParadoxOnAction trg = new();

            Context.Current.OnActions.Add(trg);
            OnActions.Add(trg);
        }

        [RelayCommand]
        private void RemoveOnAction(ParadoxOnAction trg)
        {
            OnActions.Remove(trg);
        }

        [RelayCommand]
        private async void FindOnAction()
        {
            FindOnActionDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.OnActions,
                Blacklist = new(OnActions),
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

            OnActions.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateTrigger()
        {
            ParadoxTrigger trg = new();

            Context.Current.Triggers.Add(trg);
            Triggers.Add(trg);
        }

        [RelayCommand]
        private void RemoveTrigger(ParadoxTrigger trg)
        {
            Triggers.Remove(trg);
        }

        [RelayCommand]
        private async void FindTrigger()
        {
            FindTriggerDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Triggers,
                Blacklist = new(Triggers),
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

            Triggers.Add(vm.Selected);
        }

        [RelayCommand]
        private void CreateEffect()
        {
            ParadoxEffect eff = new();

            Context.Current.Effects.Add(eff);
            Effects.Add(eff);
        }

        [RelayCommand]
        private void RemoveEffect(ParadoxEffect eff)
        {
            Effects.Remove(eff);
        }

        [RelayCommand]
        private async void FindEffect()
        {
            FindEffectDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = Context.Current.Effects,
                Blacklist = new(Effects),
            };
            FindEffectDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            Effects.Add(vm.Selected);
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Windows;
using Paradoxical.Model;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public InfoPageViewModel InfoPageViewModel { get; } = new();
        public EventPageViewModel EventPageViewModel { get; } = new();
        public TriggerPageViewModel TriggerPageViewModel { get; } = new();
        public EffectPageViewModel EffectPageViewModel { get; } = new();

        public IEnumerable<ObservableObject> Pages { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveModCommand))]
        private ParadoxMod? activeMod;

        [ObservableProperty]
        private ObservableObject? selectedPage;

        public MainViewModel()
        {
            Pages = new List<ObservableObject>()
            {
                InfoPageViewModel,
                EventPageViewModel,
                TriggerPageViewModel,
                EffectPageViewModel,
            };

            SelectedPage = EventPageViewModel;
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

            InfoPageViewModel.ActiveMod = ActiveMod;
            EventPageViewModel.ActiveMod = ActiveMod;
            TriggerPageViewModel.ActiveMod = ActiveMod;
            EffectPageViewModel.ActiveMod = ActiveMod;
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

        [RelayCommand(CanExecute = nameof(CanExportMod))]
        private void ExportMod()
        {
            throw new NotImplementedException();
        }
        private bool CanExportMod()
        {
            return ActiveMod != null;
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
    }
}
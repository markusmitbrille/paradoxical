using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Paradoxical.Data;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveModCommand))]
        private ModContext? context;

        public ObservableCollection<PageViewModel> Pages { get; } = new();

        [ObservableProperty]
        private InfoPageViewModel? infoPage;
        [ObservableProperty]
        private EventPageViewModel? eventPage;
        [ObservableProperty]
        private TriggerPageViewModel? triggerPage;
        [ObservableProperty]
        private EffectPageViewModel? effectPage;

        [ObservableProperty]
        private ObservableObject? selectedPage;

        public MainViewModel()
        {
            ResetContext();
        }

        private void ResetContext()
        {
            Context = new();

            InfoPage = new(Context);
            EventPage = new(Context);
            TriggerPage = new(Context);
            EffectPage = new(Context);

            Pages.Clear();
            Pages.Add(InfoPage);
            Pages.Add(EventPage);
            Pages.Add(TriggerPage);
            Pages.Add(EffectPage);
        }

        [RelayCommand]
        private void NewMod()
        {
            if (Context != null && MessageBox.Show(
                "Do you want to continue? Unsaved changes will be discarded!",
                "New Mod",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }

            ResetContext();
        }

        [RelayCommand]
        private void OpenMod()
        {
            if (Context != null && MessageBox.Show(
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
            return Context != null;
        }

        [RelayCommand(CanExecute = nameof(CanExportMod))]
        private void ExportMod()
        {
            throw new NotImplementedException();
        }
        private bool CanExportMod()
        {
            return Context != null;
        }

        [RelayCommand]
        private void Exit()
        {
            if (Context != null && MessageBox.Show(
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Paradoxical.Data;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveModCommand))]
        private ModContext? context;

        public ObservableCollection<PageViewModel> Pages { get; } = new();

        [ObservableProperty]
        private AboutPageViewModel? aboutPage;

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
            AboutPage = new();

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

            // navigate to info page
            SelectedPage = InfoPage;
        }

        [RelayCommand]
        private void NewMod()
        {
            if (Context != null && MessageBox.Show(
                "Are you sure?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

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

        [RelayCommand]
        private void GoToAboutPage()
        {
            if (AboutPage != null)
            {
                SelectedPage = AboutPage;
            }
        }

        [RelayCommand]
        private void GoToInfoPage()
        {
            if (InfoPage != null)
            {
                SelectedPage = InfoPage;
            }
        }

        [RelayCommand]
        private void GoToEventPage()
        {
            if (EventPage != null)
            {
                SelectedPage = EventPage;
            }
        }

        [RelayCommand]
        private void GoToTriggerPage()
        {
            if (TriggerPage != null)
            {
                SelectedPage = TriggerPage;
            }
        }

        [RelayCommand]
        private void GoToEffectPage()
        {
            if (EffectPage != null)
            {
                SelectedPage = EffectPage;
            }
        }
    }
}
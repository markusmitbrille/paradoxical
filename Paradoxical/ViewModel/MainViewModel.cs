using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Paradoxical.Data;
using Paradoxical.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveModCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveModAsCommand))]
        [NotifyCanExecuteChangedFor(nameof(BuildModCommand))]
        [NotifyCanExecuteChangedFor(nameof(BuildModAsCommand))]
        private ModContext? context;

        public ObservableCollection<PageViewModelBase> Pages { get; } = new();

        [ObservableProperty]
        private AboutPageViewModel? aboutPage;

        [ObservableProperty]
        private InfoPageViewModel? infoPage;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToEventCommand))]
        private EventPageViewModel? eventPage;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToTriggerCommand))]
        private TriggerPageViewModel? triggerPage;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToEffectCommand))]
        private EffectPageViewModel? effectPage;

        [ObservableProperty]
        private PageViewModelBase? selectedPage;

        private string ModDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string ModFile { get; set; } = string.Empty;

        public MainViewModel()
        {
            ResetContext();
        }

        private void ResetContext()
        {
            AboutPage ??= new();

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
            Pages.Add(AboutPage);

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

        [RelayCommand(CanExecute = nameof(CanSaveModAs))]
        private void SaveModAs()
        {
            throw new NotImplementedException();
        }
        private bool CanSaveModAs()
        {
            return Context != null;
        }

        [RelayCommand(CanExecute = nameof(CanBuildMod))]
        private void BuildMod()
        {
            if (Context == null)
            { return; }

            if (Directory.Exists(ModDir) == false || ModFile == string.Empty)
            {
                BuildModAs();
                return;
            }

            Context.Export(ModDir, ModFile);
        }
        private bool CanBuildMod()
        {
            return Context != null;
        }

        [RelayCommand(CanExecute = nameof(CanBuildModAs))]
        private void BuildModAs()
        {
            if (Context == null)
            { return; }

            SaveFileDialog dlg = new()
            {
                Title = "Build Mod",
                CreatePrompt = false,
                OverwritePrompt = true,
                Filter = "Paradox Mod File|*.mod",
                DefaultExt = ".mod",
                AddExtension = true,
                InitialDirectory = ModDir,
                FileName = ModFile,
            };

            if (dlg.ShowDialog() != true)
            { return; }

            ModDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ModFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;

            if (Directory.Exists(ModDir) == false)
            { return; }

            if (ModFile == string.Empty)
            { return; }

            Context.Export(ModDir, ModFile);

            Process.Start("explorer.exe", ModDir);
        }
        private bool CanBuildModAs()
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
            if (AboutPage == null)
            { return; }

            SelectedPage = AboutPage;
        }

        [RelayCommand]
        private void GoToInfoPage()
        {
            if (InfoPage == null)
            { return; }

            SelectedPage = InfoPage;
        }

        [RelayCommand]
        private void GoToEventPage()
        {
            if (EventPage == null)
            { return; }

            SelectedPage = EventPage;
        }

        [RelayCommand]
        private void GoToTriggerPage()
        {
            if (TriggerPage == null)
            { return; }

            SelectedPage = TriggerPage;
        }

        [RelayCommand]
        private void GoToEffectPage()
        {
            if (EffectPage == null)
            { return; }

            SelectedPage = EffectPage;
        }

        [RelayCommand(CanExecute = nameof(CanGoToEvent))]
        private void GoToEvent(ParadoxEvent evt)
        {
            if (EventPage == null)
            { return; }

            SelectedPage = EventPage;
            EventPage.SelectedEvent = evt;
        }
        private bool CanGoToEvent(ParadoxEvent evt)
        {
            if (EventPage == null)
            { return false; }

            if (SelectedPage == EventPage && EventPage.SelectedEvent == evt)
            { return false; }

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanGoToTrigger))]
        private void GoToTrigger(ParadoxTrigger trg)
        {
            if (TriggerPage == null)
            { return; }

            SelectedPage = TriggerPage;
            TriggerPage.SelectedTrigger = trg;
        }
        private bool CanGoToTrigger(ParadoxTrigger trg)
        {
            if (TriggerPage == null)
            { return false; }

            if (SelectedPage == TriggerPage && TriggerPage.SelectedTrigger == trg)
            { return false; }

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanGoToEffect))]
        private void GoToEffect(ParadoxEffect eff)
        {
            if (EffectPage == null)
            { return; }

            SelectedPage = EffectPage;
            EffectPage.SelectedEffect = eff;
        }
        private bool CanGoToEffect(ParadoxEffect eff)
        {
            if (EffectPage == null)
            { return false; }

            if (SelectedPage == EffectPage && EffectPage.SelectedEffect == eff)
            { return false; }

            return true;
        }
    }
}
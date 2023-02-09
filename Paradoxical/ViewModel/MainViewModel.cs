using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Paradoxical.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace Paradoxical.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<PageViewModelBase> Pages { get; } = new();

        [ObservableProperty]
        private AboutPageViewModel aboutPage = new();

        [ObservableProperty]
        private InfoPageViewModel infoPage = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToEventCommand))]
        private EventPageViewModel eventPage = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToTriggerCommand))]
        private TriggerPageViewModel triggerPage = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToEffectCommand))]
        private EffectPageViewModel effectPage = new();

        [ObservableProperty]
        private PageViewModelBase selectedPage;

        private string SaveDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string SaveFile { get; set; } = string.Empty;
        private string SavePath { get; set; } = string.Empty;

        private string ModDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string ModFile { get; set; } = string.Empty;

        public MainViewModel()
        {
            selectedPage = infoPage;
        }

        private void Reset()
        {
            InfoPage = new();
            EventPage = new();
            TriggerPage = new();
            EffectPage = new();

            Pages.Clear();
            Pages.Add(InfoPage);
            Pages.Add(EventPage);
            Pages.Add(TriggerPage);
            Pages.Add(EffectPage);
            Pages.Add(AboutPage);

            // navigate to info page
            SelectedPage = InfoPage;
        }

        private void Save(string path)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    WriteIndented = true,
                };

                string json = JsonSerializer.Serialize(Context.Current, options);
                File.WriteAllText(path, json);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not save mod!",
                    "Save Mod",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.Yes);

                throw;
            }
        }

        private void Load(string path)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    WriteIndented = true,
                };

                string json = File.ReadAllText(path);
                Context.Current = JsonSerializer.Deserialize<Context>(json, options) ?? new();

                Reset();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load mod!",
                    "Load Mod",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.Yes);

                throw;
            }
        }

        [RelayCommand]
        private void NewMod()
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Exit Application",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            Reset();
        }

        [RelayCommand]
        private void OpenMod()
        {
            if (MessageBox.Show(
                "Do you want to continue? Unsaved changes will be discarded!",
                "New Mod",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }

            OpenFileDialog dlg = new()
            {
                Title = "Open Mod",
                Filter = "Paradoxical Mod|*.paradoxical",
                DefaultExt = ".paradoxical",
                AddExtension = true,
                InitialDirectory = SaveDir,
                FileName = SaveFile,
            };

            if (dlg.ShowDialog() != true)
            { return; }

            SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
            SavePath = dlg.FileName;

            if (File.Exists(SavePath) == false)
            { return; }

            Load(SavePath);
        }

        [RelayCommand]
        private void SaveMod()
        {
            if (File.Exists(SavePath) == false)
            {
                SaveModAs();
                return;
            }

            Save(SavePath);
        }

        [RelayCommand]
        private void SaveModAs()
        {
            SaveFileDialog dlg = new()
            {
                Title = "Save Mod",
                CreatePrompt = false,
                OverwritePrompt = true,
                Filter = "Paradoxical Mod|*.paradoxical",
                DefaultExt = ".paradoxical",
                AddExtension = true,
                InitialDirectory = SaveDir,
                FileName = SaveFile,
            };

            if (dlg.ShowDialog() != true)
            { return; }

            SaveDir = Path.GetDirectoryName(dlg.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            SaveFile = Path.GetFileNameWithoutExtension(dlg.FileName) ?? string.Empty;
            SavePath = dlg.FileName;

            if (Directory.Exists(SaveDir) == false)
            { return; }

            if (SaveFile == string.Empty)
            { return; }

            Save(SavePath);
        }

        [RelayCommand]
        private void BuildMod()
        {
            if (Directory.Exists(ModDir) == false || ModFile == string.Empty)
            {
                BuildModAs();
                return;
            }

            Context.Current.Export(ModDir, ModFile);
        }

        [RelayCommand]
        private void BuildModAs()
        {
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

            Context.Current.Export(ModDir, ModFile);

            Process.Start("explorer.exe", ModDir);
        }

        [RelayCommand]
        private void Exit()
        {
            if (MessageBox.Show(
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
            SelectedPage = EventPage;
            EventPage.SelectedEvent = evt;
        }
        private bool CanGoToEvent(ParadoxEvent evt)
        {
            if (SelectedPage == EventPage && EventPage.SelectedEvent == evt)
            { return false; }

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanGoToTrigger))]
        private void GoToTrigger(ParadoxTrigger trg)
        {
            SelectedPage = TriggerPage;
            TriggerPage.SelectedTrigger = trg;
        }
        private bool CanGoToTrigger(ParadoxTrigger trg)
        {
            if (SelectedPage == TriggerPage && TriggerPage.SelectedTrigger == trg)
            { return false; }

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanGoToEffect))]
        private void GoToEffect(ParadoxEffect eff)
        {
            SelectedPage = EffectPage;
            EffectPage.SelectedEffect = eff;
        }
        private bool CanGoToEffect(ParadoxEffect eff)
        {
            if (SelectedPage == EffectPage && EffectPage.SelectedEffect == eff)
            { return false; }

            return true;
        }
    }
}
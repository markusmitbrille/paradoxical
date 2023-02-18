using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Paradoxical.Model;
using Paradoxical.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private EventPageViewModel eventPage = new();
        [ObservableProperty]
        private DecisionPageViewModel decisionPage = new();
        [ObservableProperty]
        private OnActionPageViewModel onActionPage = new();
        [ObservableProperty]
        private TriggerPageViewModel triggerPage = new();
        [ObservableProperty]
        private EffectPageViewModel effectPage = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedPage))]
        [NotifyPropertyChangedFor(nameof(SelectedElement))]
        private ObservableObject? selectedObject;

        public PageViewModelBase? SelectedPage
        {
            get => SelectedObject as PageViewModelBase;
            set => SelectedObject = value;
        }
        public ParadoxElement? SelectedElement
        {
            get => SelectedObject as ParadoxElement;
            set => SelectedObject = value;
        }

        private string SaveDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string SaveFile { get; set; } = string.Empty;
        private string SavePath { get; set; } = string.Empty;

        private string ModDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string ModFile { get; set; } = string.Empty;

        private readonly Stack<ObservableObject> history = new();
        private readonly Stack<ObservableObject> future = new();

        public MainViewModel()
        {
            Pages.Add(infoPage);
            Pages.Add(eventPage);
            Pages.Add(decisionPage);
            Pages.Add(onActionPage);
            Pages.Add(triggerPage);
            Pages.Add(effectPage);
            Pages.Add(aboutPage);

            // navigate to info page
            selectedObject = infoPage;
        }

        private void Reset()
        {
            GoToNone();

            InfoPage = new();
            EventPage = new();
            DecisionPage = new();
            OnActionPage = new();
            TriggerPage = new();
            EffectPage = new();

            Pages.Clear();
            Pages.Add(InfoPage);
            Pages.Add(EventPage);
            Pages.Add(DecisionPage);
            Pages.Add(OnActionPage);
            Pages.Add(TriggerPage);
            Pages.Add(EffectPage);
            Pages.Add(AboutPage);

            // navigate to info page
            GoToPage(InfoPage);
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

        [RelayCommand(CanExecute = nameof(CanNavigateNext))]
        private void NavigateNext()
        {
            if (SelectedObject != null)
            {
                history.Push(SelectedObject);
            }

            SelectedObject = future.Pop();

            NavigateNextCommand.NotifyCanExecuteChanged();
            NavigatePreviousCommand.NotifyCanExecuteChanged();
        }
        private bool CanNavigateNext()
        {
            return future.Any();
        }

        [RelayCommand(CanExecute = nameof(CanNavigatePrevious))]
        private void NavigatePrevious()
        {
            if (SelectedObject != null)
            {
                future.Push(SelectedObject);
            }

            SelectedObject = history.Pop();

            NavigateNextCommand.NotifyCanExecuteChanged();
            NavigatePreviousCommand.NotifyCanExecuteChanged();
        }
        private bool CanNavigatePrevious()
        {
            return history.Any();
        }

        [RelayCommand]
        private void GoToNone()
        {
            history.Clear();
            future.Clear();

            SelectedObject = null;

            NavigateNextCommand.NotifyCanExecuteChanged();
            NavigatePreviousCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanGoToPage))]
        private void GoToPage(PageViewModelBase page)
        {
            future.Clear();
            if (SelectedObject != null)
            {
                history.Push(SelectedObject);
            }

            SelectedPage = page;

            NavigateNextCommand.NotifyCanExecuteChanged();
            NavigatePreviousCommand.NotifyCanExecuteChanged();
        }
        private bool CanGoToPage(PageViewModelBase page)
        {
            if (SelectedPage == page)
            { return false; }

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanGoToElement))]
        private void GoToElement(ParadoxElement element)
        {
            future.Clear();
            if (SelectedObject != null)
            {
                history.Push(SelectedObject);
            }

            SelectedElement = element;

            NavigateNextCommand.NotifyCanExecuteChanged();
            NavigatePreviousCommand.NotifyCanExecuteChanged();
        }
        private bool CanGoToElement(ParadoxElement element)
        {
            if (SelectedElement == element)
            { return false; }

            return true;
        }

        [RelayCommand]
        private async void Find()
        {
            var elements = Enumerable.Empty<ParadoxElement>()
                .Union(Context.Current.Events)
                .Union(Context.Current.Triggers)
                .Union(Context.Current.Effects)
                .Union(Context.Current.OnActions)
                .Union(Context.Current.Decisions);

            FindDialogViewModel vm = new()
            {
                DialogIdentifier = MainWindow.ROOT_DIALOG_IDENTIFIER,
                Items = new(elements),
            };
            FindDialogView dlg = new()
            {
                DataContext = vm,
            };

            await DialogHost.Show(dlg, MainWindow.ROOT_DIALOG_IDENTIFIER);

            if (vm.DialogResult != true)
            { return; }

            if (vm.Selected == null)
            { return; }

            ParadoxElement selected = vm.Selected;
            if (CanGoToElement(selected))
            {
                GoToElement(selected);
            }
        }

        [RelayCommand]
        private void AddEvent()
        {
            ParadoxEvent element = new();

            if (CanGoToElement(element))
            { GoToElement(element); }
        }

        [RelayCommand]
        private void DuplicateEvent(ParadoxEvent element)
        {
            ParadoxEvent other = new(element);

            if (CanGoToElement(other))
            { GoToElement(other); }
        }

        [RelayCommand]
        private void RemoveEvent(ParadoxEvent element)
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Remove Event",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            element.Delete();

            if (SelectedElement == element)
            { GoToNone(); }
        }

        [RelayCommand]
        private void AddDecision()
        {
            ParadoxDecision element = new();

            if (CanGoToElement(element))
            { GoToElement(element); }
        }

        [RelayCommand]
        private void DuplicateDecision(ParadoxDecision element)
        {
            ParadoxDecision other = new(element);

            if (CanGoToElement(other))
            { GoToElement(other); }
        }

        [RelayCommand]
        private void RemoveDecision(ParadoxDecision element)
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Remove Decision",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            element.Delete();

            if (SelectedElement == element)
            { GoToNone(); }
        }

        [RelayCommand]
        private void AddOnAction()
        {
            ParadoxOnAction element = new();

            if (CanGoToElement(element))
            { GoToElement(element); }
        }

        [RelayCommand]
        private void DuplicateOnAction(ParadoxOnAction element)
        {
            ParadoxOnAction other = new(element);

            if (CanGoToElement(other))
            { GoToElement(other); }
        }

        [RelayCommand]
        private void RemoveOnAction(ParadoxOnAction element)
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Remove On-Action",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            element.Delete();

            if (SelectedElement == element)
            { GoToNone(); }
        }

        [RelayCommand]
        private void AddTrigger()
        {
            ParadoxTrigger element = new();

            if (CanGoToElement(element))
            { GoToElement(element); }
        }

        [RelayCommand]
        private void DuplicateTrigger(ParadoxTrigger element)
        {
            ParadoxTrigger other = new(element);

            if (CanGoToElement(other))
            { GoToElement(other); }
        }

        [RelayCommand]
        private void RemoveTrigger(ParadoxTrigger element)
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Remove Trigger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            element.Delete();

            if (SelectedElement == element)
            { GoToNone(); }
        }

        [RelayCommand]
        private void AddEffect()
        {
            ParadoxEffect element = new();

            if (CanGoToElement(element))
            { GoToElement(element); }
        }

        [RelayCommand]
        private void DuplicateEffect(ParadoxEffect element)
        {
            ParadoxEffect other = new(element);

            if (CanGoToElement(other))
            { GoToElement(other); }
        }

        [RelayCommand]
        private void RemoveEffect(ParadoxEffect element)
        {
            if (MessageBox.Show(
                "Are you sure?",
                "Remove Effect",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.Yes) != MessageBoxResult.Yes)
            { return; }

            element.Delete();

            if (SelectedElement == element)
            {
                SelectedElement = Context.Current.Effects.FirstOrDefault();
            }
        }
    }
}
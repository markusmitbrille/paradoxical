using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Paradoxical.Model
{
    public partial class Context : ObservableObject
    {
        private const string EVENTS_DIR = "events";
        private const string COMMON_DIR = "common";
        private const string SCRIPTED_TRIGGERS_DIR = "common/scripted_triggers";
        private const string SCRIPTED_EFFECTS_DIR = "common/scripted_effects";
        private const string ON_ACTION_DIR = "common/on_action";
        private const string DECISIONS_DIR = "common/decisions";
        private const string LOCALIZATION_DIR = "localization";
        private const string LOCALIZATION_ENGLISH_DIR = "localization/english";

        public static Context Current { get; set; } = new();

        [ObservableProperty]
        private ParadoxMod info = new();

        [ObservableProperty]
        private ObservableCollection<ParadoxEvent> events = new();
        [ObservableProperty]
        private ObservableCollection<ParadoxTrigger> triggers = new();
        [ObservableProperty]
        private ObservableCollection<ParadoxEffect> effects = new();
        [ObservableProperty]
        private ObservableCollection<ParadoxOnAction> onActions = new();
        [ObservableProperty]
        private ObservableCollection<ParadoxDecision> decisions = new();

        public string EventsFile => $"{Info.EventNamespace}_events.txt";
        public string TriggersFile => $"{Info.EventNamespace}_triggers.txt";
        public string EffectsFile => $"{Info.EventNamespace}_effects.txt";
        public string OnActionsFile => $"{Info.EventNamespace}_on_actions.txt";
        public string DecisionsFile => $"{Info.EventNamespace}_decisions.txt";
        public string LocFile => $"{Info.EventNamespace}_l_english.yml";

        public void Export(string dir, string file)
        {
            UTF8Encoding encoding = new(true);
            FileStreamOptions options = new()
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
            };

            Directory.CreateDirectory(GetModDir(dir, file));
            Directory.CreateDirectory(GetEventsDir(dir, file));
            Directory.CreateDirectory(GetCommonDir(dir, file));
            Directory.CreateDirectory(GetScriptedTriggersDir(dir, file));
            Directory.CreateDirectory(GetScriptedEffectsDir(dir, file));
            Directory.CreateDirectory(GetOnActionDir(dir, file));
            Directory.CreateDirectory(GetDecisionsDir(dir, file));
            Directory.CreateDirectory(GetLocDir(dir, file));
            Directory.CreateDirectory(GetEnglishLocDir(dir, file));

            using (StreamWriter writer = new(GetModFilePath(dir, file), encoding, options))
            {
                WriteModFile(writer, dir, file);
            }

            using (StreamWriter writer = new(GetDescriptorFilePath(dir, file), encoding, options))
            {
                WriteModFile(writer, dir, file);
            }

            using (StreamWriter writer = new(GetEventsFilePath(dir, file), encoding, options))
            {
                WriteEventsFile(writer);
            }

            using (StreamWriter writer = new(GetTriggersFilePath(dir, file), encoding, options))
            {
                WriteTriggersFile(writer);
            }

            using (StreamWriter writer = new(GetEffectsFilePath(dir, file), encoding, options))
            {
                WriteEffectsFile(writer);
            }

            using (StreamWriter writer = new(GetOnActionsFilePath(dir, file), encoding, options))
            {
                WriteOnActionsFile(writer);
            }

            using (StreamWriter writer = new(GetDecisionsFilePath(dir, file), encoding, options))
            {
                WriteDecisionsFile(writer);
            }

            using (StreamWriter writer = new(GetLocFilePath(dir, file), encoding, options))
            {
                WriteLocFile(writer);
            }
        }

        private static string GetModDir(string dir, string file)
        {
            return Path.Combine(dir, file);
        }

        private static string GetEventsDir(string dir, string file)
        {
            return Path.Combine(dir, file, EVENTS_DIR);
        }

        private static string GetCommonDir(string dir, string file)
        {
            return Path.Combine(dir, file, COMMON_DIR);
        }

        private static string GetScriptedTriggersDir(string dir, string file)
        {
            return Path.Combine(dir, file, SCRIPTED_TRIGGERS_DIR);
        }

        private static string GetScriptedEffectsDir(string dir, string file)
        {
            return Path.Combine(dir, file, SCRIPTED_EFFECTS_DIR);
        }

        private static string GetOnActionDir(string dir, string file)
        {
            return Path.Combine(dir, file, ON_ACTION_DIR);
        }

        private static string GetDecisionsDir(string dir, string file)
        {
            return Path.Combine(dir, file, DECISIONS_DIR);
        }

        private static string GetLocDir(string dir, string file)
        {
            return Path.Combine(dir, file, LOCALIZATION_DIR);
        }

        private static string GetEnglishLocDir(string dir, string file)
        {
            return Path.Combine(dir, file, LOCALIZATION_ENGLISH_DIR);
        }

        private static string GetModFilePath(string dir, string file)
        {
            return Path.Combine(dir, $"{file}.mod");
        }

        private static string GetDescriptorFilePath(string dir, string file)
        {
            return Path.Combine(dir, file, "descriptor.mod");
        }

        private string GetEventsFilePath(string dir, string file)
        {
            return Path.Combine(GetEventsDir(dir, file), EventsFile);
        }

        private string GetTriggersFilePath(string dir, string file)
        {
            return Path.Combine(GetScriptedTriggersDir(dir, file), TriggersFile);
        }

        private string GetEffectsFilePath(string dir, string file)
        {
            return Path.Combine(GetScriptedEffectsDir(dir, file), EffectsFile);
        }

        private string GetOnActionsFilePath(string dir, string file)
        {
            return Path.Combine(GetOnActionDir(dir, file), OnActionsFile);
        }

        private string GetDecisionsFilePath(string dir, string file)
        {
            return Path.Combine(GetDecisionsDir(dir, file), DecisionsFile);
        }

        private string GetLocFilePath(string dir, string file)
        {
            return Path.Combine(GetEnglishLocDir(dir, file), LocFile);
        }

        private void WriteModFile(TextWriter writer, string dir, string file)
        {
            if (Info == null)
            { return; }

            Info.Write(writer, dir, file);
        }

        private void WriteEventsFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Events");
            writer.WriteLine($"namespace = {Info.EventNamespace}");
            writer.WriteLine();

            foreach (ParadoxEvent evt in Events)
            {
                ParadoxText.IndentLevel = 0;

                evt.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteTriggersFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Triggers");

            foreach (ParadoxTrigger trg in Triggers)
            {
                ParadoxText.IndentLevel = 0;

                trg.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteEffectsFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Effects");

            foreach (ParadoxEffect eff in Effects)
            {
                ParadoxText.IndentLevel = 0;

                eff.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteOnActionsFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} On-Actions");

            foreach (ParadoxOnAction act in OnActions)
            {
                ParadoxText.IndentLevel = 0;

                act.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteDecisionsFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Decisions");

            foreach (ParadoxDecision dec in Decisions)
            {
                ParadoxText.IndentLevel = 0;

                dec.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteLocFile(TextWriter writer)
        {
            writer.WriteLine("l_english:");
            writer.WriteLine();

            writer.WriteLine("# triggers");

            foreach (ParadoxTrigger trg in Triggers)
            {
                trg.WriteLoc(writer);
                writer.WriteLine();
            }

            writer.WriteLine("# effects");

            foreach (ParadoxEffect eff in Effects)
            {
                eff.WriteLoc(writer);
                writer.WriteLine();
            }

            writer.WriteLine("# events");

            foreach (ParadoxEvent evt in Events)
            {
                evt.WriteLoc(writer);
                writer.WriteLine();
            }

            writer.WriteLine("# decisions");

            foreach (ParadoxDecision dec in Decisions)
            {
                dec.WriteLoc(writer);
                writer.WriteLine();
            }
        }
    }
}

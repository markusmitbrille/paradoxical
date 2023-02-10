using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Paradoxical.Model
{
    public partial class Context : ObservableObject
    {
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

        public string EventFileEntryName => $"events/{Info.EventNamespace}_events.txt";
        public string TriggerFileEntryName => $"common/scripted_triggers/{Info.EventNamespace}_triggers.txt";
        public string EffectFileEntryName => $"common/scripted_effects/{Info.EventNamespace}_effects.txt";
        public string LocalizationFileEntryName => $"localization/english/{Info.EventNamespace}_l_english.yml";

        public void Export(string dir, string file)
        {
            using (StreamWriter writer = new(Path.Combine(dir, $"{file}.mod"), false))
            {
                WriteModFile(writer, dir, file);
            }

            using FileStream stream = new(Path.Combine(dir, $"{file}.zip"), FileMode.Create);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update, false, Encoding.UTF8);

            ZipArchiveEntry descriptorEntry = archive.CreateEntry("descriptor.mod");
            using (StreamWriter writer = new(descriptorEntry.Open()))
            {
                WriteModFile(writer, dir, file);
            }

            ZipArchiveEntry eventEntry = archive.CreateEntry(EventFileEntryName);
            using (StreamWriter writer = new(eventEntry.Open()))
            {
                WriteEventFile(writer);
            }

            ZipArchiveEntry triggerEntry = archive.CreateEntry(TriggerFileEntryName);
            using (StreamWriter writer = new(triggerEntry.Open()))
            {
                WriteTriggerFile(writer);
            }

            ZipArchiveEntry effectEntry = archive.CreateEntry(EffectFileEntryName);
            using (StreamWriter writer = new(effectEntry.Open()))
            {
                WriteEffectFile(writer);
            }

            ZipArchiveEntry localizationEntry = archive.CreateEntry(LocalizationFileEntryName);
            using (StreamWriter writer = new(localizationEntry.Open()))
            {
                WriteLocFile(writer);
            }
        }

        private void WriteModFile(TextWriter writer, string dir, string file)
        {
            if (Info == null)
            { return; }

            Info.Write(writer, dir, file);
        }

        private void WriteEventFile(TextWriter writer)
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

        private void WriteTriggerFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Triggers");

            foreach (ParadoxTrigger trg in Triggers)
            {
                ParadoxText.IndentLevel = 0;

                trg.Write(writer);
                writer.WriteLine();
            }
        }

        private void WriteEffectFile(TextWriter writer)
        {
            writer.WriteLine($"# {Info.Name} Effects");

            foreach (ParadoxEffect eff in Effects)
            {
                ParadoxText.IndentLevel = 0;

                eff.Write(writer);
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
        }
    }
}

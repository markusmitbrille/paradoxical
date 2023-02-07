using Paradoxical.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Paradoxical.Data
{
    public class ModContext
    {
        public ParadoxMod Info { get; }

        public ObservableCollection<ParadoxEvent> Events { get; } = new();
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();
        public ObservableCollection<ParadoxEffect> Effects { get; } = new();

        public string Prefix => Info.EventNamespace.Trim().Replace(" ", "_");

        public string EventFileEntryName => $"events/{Prefix}_events.txt";
        public string TriggerFileEntryName => $"common/scripted_triggers/{Prefix}_triggers.txt";
        public string EffectFileEntryName => $"common/scripted_effects/{Prefix}_effects.txt";
        public string LocalizationFileEntryName => $"localization/english/{Prefix}_l_english.yml";

        public ModContext()
        {
            Info = new(this);
        }

        public void Export(string dir, string file)
        {
            using FileStream stream = new(Path.Combine(dir, $"{file}.zip"), FileMode.Create);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update, false, Encoding.UTF8);

            ZipArchiveEntry descriptorEntry = archive.CreateEntry("descriptor.mod");
            using (StreamWriter writer = new(descriptorEntry.Open()))
            {
                writer.Write(CompileModFile());
            }

            ZipArchiveEntry eventEntry = archive.CreateEntry(EventFileEntryName);
            using (StreamWriter writer = new(eventEntry.Open()))
            {
                writer.Write(CompileEventFile());
            }

            ZipArchiveEntry triggerEntry = archive.CreateEntry(TriggerFileEntryName);
            using (StreamWriter writer = new(triggerEntry.Open()))
            {
                writer.Write(CompileTriggerFile());
            }

            ZipArchiveEntry effectEntry = archive.CreateEntry(EffectFileEntryName);
            using (StreamWriter writer = new(effectEntry.Open()))
            {
                writer.Write(CompileEffectFile());
            }

            ZipArchiveEntry localizationEntry = archive.CreateEntry(LocalizationFileEntryName);
            using (StreamWriter writer = new(localizationEntry.Open()))
            {
                writer.Write(CompileLocalizationFile());
            }

            File.WriteAllText(Path.Combine(dir, $"{file}.mod"), CompileModFile());
        }

        private string CompileModFile()
        {
            return "Mod File Content";
        }

        private string CompileEventFile()
        {
            return "Event File Content";
        }

        private string CompileTriggerFile()
        {
            return "Trigger File Content";
        }

        private string CompileEffectFile()
        {
            return "Effect File Content";
        }

        private string CompileLocalizationFile()
        {
            return "Localization File Content";
        }
    }
}

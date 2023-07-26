using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System.IO;
using System.Linq;
using System.Text;

namespace Paradoxical.Services;

public interface IBuildService
{
    void Export(string dir, string file);

    void WriteEventsFile(TextWriter writer);
    void WriteTriggersFile(TextWriter writer);
    void WriteEffectsFile(TextWriter writer);
    void WriteLocFile(TextWriter writer);
}

public class BuildService : IBuildService
{
    private const string EVENTS_DIR = "events";
    private const string COMMON_DIR = "common";
    private const string SCRIPTED_TRIGGERS_DIR = "common/scripted_triggers";
    private const string SCRIPTED_EFFECTS_DIR = "common/scripted_effects";
    private const string ON_ACTION_DIR = "common/on_action";
    private const string DECISIONS_DIR = "common/decisions";
    private const string LOCALIZATION_DIR = "localization";
    private const string LOCALIZATION_ENGLISH_DIR = "localization/english";

    private string EventsFile => $"{modService.GetPrefix()}_events.txt";
    private string TriggersFile => $"{modService.GetPrefix()}_triggers.txt";
    private string EffectsFile => $"{modService.GetPrefix()}_effects.txt";
    private string LocFile => $"{modService.GetPrefix()}_l_english.yml";

    private readonly IModService modService;

    private readonly IScriptService scriptService;

    private readonly IEventService eventService;
    private readonly IOptionService optionService;
    private readonly IPortraitService portraitService;
    private readonly ITriggerService triggerService;
    private readonly IEffectService effectService;

    public BuildService(
        IModService modService,
        IScriptService scriptService,
        IEventService eventService,
        IOptionService optionService,
        IPortraitService portraitService,
        ITriggerService triggerService,
        IEffectService effectService)
    {
        this.modService = modService;

        this.scriptService = scriptService;

        this.eventService = eventService;
        this.optionService = optionService;
        this.portraitService = portraitService;
        this.triggerService = triggerService;
        this.effectService = effectService;
    }

    public void Export(string dir, string file)
    {
        UTF8Encoding encoding = new(true);
        FileStreamOptions options = new()
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
        };

        Directory.Delete(GetModDir(dir, file), true);

        Directory.CreateDirectory(GetModDir(dir, file));
        Directory.CreateDirectory(GetEventsDir(dir, file));
        Directory.CreateDirectory(GetCommonDir(dir, file));
        Directory.CreateDirectory(GetScriptedTriggersDir(dir, file));
        Directory.CreateDirectory(GetScriptedEffectsDir(dir, file));
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

        using (StreamWriter writer = new(GetLocFilePath(dir, file), encoding, options))
        {
            WriteLocFile(writer);
        }

        WriteScriptFiles(dir, file);
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

    private string GetLocFilePath(string dir, string file)
    {
        return Path.Combine(GetEnglishLocDir(dir, file), LocFile);
    }

    private void WriteModFile(TextWriter writer, string dir, string file)
    {
        var mod = modService.Get().SingleOrDefault() ?? new();
        mod.Write(writer, dir, file);
    }

    private void WriteScriptFiles(string dir, string file)
    {
        UTF8Encoding encoding = new(true);
        FileStreamOptions options = new()
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
        };

        foreach (Script script in scriptService.Get())
        {
            string sdir = Path.Combine(dir, file, script.dir);
            string sfile = Path.Combine(dir, file, script.dir, script.file);

            Directory.CreateDirectory(sdir);

            using StreamWriter writer = new(sfile, encoding, options);
            script.Write(writer);
        }
    }

    public void WriteEventsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Events");
        writer.WriteLine($"namespace = {modService.GetPrefix()}");
        writer.WriteLine();

        foreach (Event element in eventService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(
                writer,
                modService,
                eventService,
                optionService,
                portraitService);

            writer.WriteLine();
        }
    }

    public void WriteTriggersFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Triggers");
        writer.WriteLine();

        foreach (Trigger element in triggerService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService);
            writer.WriteLine();
        }
    }

    public void WriteEffectsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Effects");
        writer.WriteLine();

        foreach (Effect element in effectService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService);
            writer.WriteLine();
        }
    }

    public void WriteLocFile(TextWriter writer)
    {
        writer.WriteLine("l_english:");
        writer.WriteLine();

        writer.WriteLine("# triggers");

        foreach (Trigger element in triggerService.Get())
        {
            element.WriteLoc(writer, modService);
            writer.WriteLine();
        }

        writer.WriteLine("# effects");

        foreach (Effect element in effectService.Get())
        {
            element.WriteLoc(writer, modService);
            writer.WriteLine();
        }

        writer.WriteLine("# events");

        foreach (Event evt in eventService.Get())
        {
            evt.WriteLoc(writer, modService);
            writer.WriteLine();
        }

        writer.WriteLine("# options");

        foreach (Option opt in optionService.Get())
        {
            opt.WriteLoc(writer, modService);
            writer.WriteLine();
        }
    }
}

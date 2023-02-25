using Paradoxical.Model;
using System.IO;
using System.Text;

namespace Paradoxical.Services;

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
    private string OnActionsFile => $"{modService.GetPrefix()}_on_actions.txt";
    private string DecisionsFile => $"{modService.GetPrefix()}_decisions.txt";
    private string LocFile => $"{modService.GetPrefix()}_l_english.yml";

    private readonly IModService modService;
    private readonly IEventService eventService;
    private readonly IOnActionService onActionService;
    private readonly IDecisionService decisionService;
    private readonly ITriggerService triggerService;
    private readonly IEffectService effectService;
    private readonly IOptionService optionService;
    private readonly IPortraitService portraitService;

    public BuildService(
        IModService modService,
        IEventService eventService,
        IOnActionService onActionService,
        IDecisionService decisionService,
        ITriggerService triggerService,
        IEffectService effectService,
        IOptionService optionService,
        IPortraitService portraitService)
    {
        this.modService = modService;
        this.eventService = eventService;
        this.onActionService = onActionService;
        this.decisionService = decisionService;
        this.triggerService = triggerService;
        this.effectService = effectService;
        this.optionService = optionService;
        this.portraitService = portraitService;
    }

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
        modService.Get().Write(writer, dir, file);
    }

    private void WriteEventsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Events");
        writer.WriteLine($"namespace = {modService.GetPrefix()}");
        writer.WriteLine();

        foreach (Event element in eventService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService, eventService, optionService, portraitService);
            writer.WriteLine();
        }
    }

    private void WriteTriggersFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Triggers");

        foreach (Trigger element in triggerService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService);
            writer.WriteLine();
        }
    }

    private void WriteEffectsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Effects");

        foreach (Effect element in effectService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService);
            writer.WriteLine();
        }
    }

    private void WriteOnActionsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} On-Actions");

        foreach (OnAction element in onActionService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService, onActionService);
            writer.WriteLine();
        }
    }

    private void WriteDecisionsFile(TextWriter writer)
    {
        writer.WriteLine($"# {modService.GetModName()} Decisions");

        foreach (Decision element in decisionService.Get())
        {
            ParadoxText.IndentLevel = 0;

            element.Write(writer, modService, decisionService);
            writer.WriteLine();
        }
    }

    private void WriteLocFile(TextWriter writer)
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
            evt.WriteLoc(writer, modService, eventService, optionService);
            writer.WriteLine();
        }

        writer.WriteLine("# decisions");

        foreach (Decision dec in decisionService.Get())
        {
            dec.WriteLoc(writer, modService);
            writer.WriteLine();
        }
    }

}

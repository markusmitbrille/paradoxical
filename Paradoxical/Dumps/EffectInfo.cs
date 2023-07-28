using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Paradoxical.Dumps;

public partial class EffectInfo
{
    private const string LOG_FILE = "Paradoxical.Dumps.Logs.effects.log";

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? SupportedScopes { get; init; }
    public string? SupportedTargets { get; init; }

    public string Tooltip
    {
        get
        {
            string tooltip = "";
            if (Description.IsEmpty() == false)
            {
                tooltip += Description + "\r\n";
            }
            tooltip += $"{SupportedScopes ?? "global"}{(SupportedTargets != null ? $"🠆 {SupportedTargets}" : "")}";

            return tooltip;
        }
    }

    public static IEnumerable<EffectInfo> ParseLog()
    {
        return ParseLog(ReadEmbeddedResource(LOG_FILE));
    }

    static string ReadEmbeddedResource(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new Exception($"Resource '{resourceName}' not found in the assembly.");
        }

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    [GeneratedRegex(@"(?<name>\w+) - (?<desc>[^\r\n]+)")]
    private static partial Regex GetCoreRegex();
    private static Regex CoreRegex => GetCoreRegex();

    [GeneratedRegex(@"Supported Scopes: (?<scopes>(?:\w+[\s-[\r\n]]*)+)")]
    private static partial Regex GetSupportedScopesRegex();
    private static Regex SupportedScopesRegex => GetSupportedScopesRegex();

    [GeneratedRegex(@"Supported Targets: (?<targets>(?:\w+[\s-[\r\n]]*)+)")]
    private static partial Regex GetSupportedTargetsRegex();
    private static Regex SupportedTargetsRegex => GetSupportedTargetsRegex();

    public static IEnumerable<EffectInfo> ParseLog(string log)
    {
        string[] chunks = log.Split("--------------------", StringSplitOptions.TrimEntries);
        foreach (string chunk in chunks)
        {
            Match coreMatch = CoreRegex.Match(chunk);
            if (coreMatch.Success == false)
            {
                continue;
            }

            string name = coreMatch.Groups["name"].Value;
            string description = coreMatch.Groups["desc"].Value;

            string? supportedScopes = null;
            Match supportedScopesMatch = SupportedScopesRegex.Match(chunk);
            if (supportedScopesMatch.Success == true)
            {
                supportedScopes = supportedScopesMatch.Groups["scopes"].Value;
            }

            string? supportedTargets = null;
            Match supportedTargetsMatch = SupportedTargetsRegex.Match(chunk);
            if (supportedTargetsMatch.Success == true)
            {
                supportedTargets = supportedTargetsMatch.Groups["targets"].Value;
            }

            yield return new()
            {
                Name = name,
                Description = description,
                SupportedScopes = supportedScopes,
                SupportedTargets = supportedTargets,
            };
        }
    }
}

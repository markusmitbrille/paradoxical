using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradoxical.Info;

public partial class TriggerInfo
{
    private const string LOG_FILE = "Paradoxical.Resources.Logs.triggers.log";

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? SupportedScopes { get; init; }
    public string? SupportedTargets { get; init; }
    public string? Traits { get; init; }

    public string Tooltip
    {
        get
        {
            string tooltip = "";
            if (Description.IsEmpty() == false)
            {
                tooltip += Description + Environment.NewLine;
            }
            tooltip += $"{SupportedScopes ?? "global"}{(SupportedTargets != null ? $" 🠆 {SupportedTargets}" : "")}";

            return tooltip;
        }
    }

    public static IEnumerable<TriggerInfo> ParseLog()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? log = assembly.ReadEmbeddedResource(LOG_FILE);

        if (log == null)
        {
            throw new Exception($"Did not find embedded log resource '{LOG_FILE}'!");
        }

        return ParseLog(log);
    }

    [GeneratedRegex(@"(?<name>\w+) - (?<desc>[^\r\n]+)")]
    private static partial Regex GetCoreRegex();
    private static Regex CoreRegex => GetCoreRegex();

    [GeneratedRegex(@"Supported Scopes: (?<scopes>\w+(?:, \w+)*)")]
    private static partial Regex GetSupportedScopesRegex();
    private static Regex SupportedScopesRegex => GetSupportedScopesRegex();

    [GeneratedRegex(@"Supported Targets: (?<targets>\w+(?:, \w+)*)")]
    private static partial Regex GetSupportedTargetsRegex();
    private static Regex SupportedTargetsRegex => GetSupportedTargetsRegex();

    [GeneratedRegex(@"Traits: (?<traits>[^\r\n]+)")]
    private static partial Regex GetTraitsRegex();
    private static Regex TraitsRegex => GetTraitsRegex();

    public static IEnumerable<TriggerInfo> ParseLog(string log)
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

            string? traits = null;
            Match traitsMatch = TraitsRegex.Match(chunk);
            if (traitsMatch.Success == true)
            {
                traits = traitsMatch.Groups["traits"].Value;
            }

            yield return new()
            {
                Name = name,
                Description = description,
                SupportedScopes = supportedScopes,
                SupportedTargets = supportedTargets,
                Traits = traits,
            };
        }
    }
}

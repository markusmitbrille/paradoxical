using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradoxical.Info;

public partial class ScopeInfo
{
    private const string LOG_FILE = "Paradoxical.Resources.Logs.event_targets.log";

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? InputScopes { get; init; }
    public string? OutputScopes { get; init; }

    public string Tooltip
    {
        get
        {
            string tooltip = "";
            if (Description.IsEmpty() == false)
            {
                tooltip += Description + "\r\n";
            }

            tooltip += $"{InputScopes ?? "global"}{(OutputScopes != null ? $"🠆 {OutputScopes}" : "")}";

            return tooltip;
        }
    }

    public static IEnumerable<ScopeInfo> ParseLog()
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

    [GeneratedRegex(@"Input Scopes: (?<scopes>(?:\w+[\s-[\r\n]]*)+)")]
    private static partial Regex GetInputScopesRegex();
    private static Regex InputScopesRegex => GetInputScopesRegex();

    [GeneratedRegex(@"Input Scopes: (?<scopes>(?:\w+[\s-[\r\n]]*)+)")]
    private static partial Regex GetOutputScopesRegex();
    private static Regex OutputScopesRegex => GetOutputScopesRegex();

    public static IEnumerable<ScopeInfo> ParseLog(string log)
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

            string? inputScopes = null;
            Match inputScopesMatch = InputScopesRegex.Match(chunk);
            if (inputScopesMatch.Success == true)
            {
                inputScopes = inputScopesMatch.Groups["scopes"].Value;
            }

            string? outputScopes = null;
            Match outputScopesMatch = OutputScopesRegex.Match(chunk);
            if (inputScopesMatch.Success == true)
            {
                outputScopes = outputScopesMatch.Groups["scopes"].Value;
            }

            yield return new()
            {
                Name = name,
                Description = description,
                InputScopes = inputScopes,
                OutputScopes = outputScopes,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradoxical.Dumps;

public partial class OnionInfo
{
    private const string LOG_FILE = "Paradoxical.Dumps.Logs.on_actions.log";

    public string Name { get; init; } = string.Empty;
    public string? ExpectedScope { get; init; }

    public static IEnumerable<OnionInfo> ParseLog()
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

    [GeneratedRegex(@"^(?<name>\w+):")]
    private static partial Regex GetCoreRegex();
    private static Regex CoreRegex => GetCoreRegex();

    [GeneratedRegex(@"Expected Scope: (?<scope>(?:\w+[\s-[\r\n]]*)+)")]
    private static partial Regex GetExpectedScopeRegex();
    private static Regex ExpectedScopeRegex => GetExpectedScopeRegex();

    public static IEnumerable<OnionInfo> ParseLog(string log)
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

            string? expectedScope = null;
            Match expectedScopeMatch = ExpectedScopeRegex.Match(chunk);
            if (expectedScopeMatch.Success == true)
            {
                expectedScope = expectedScopeMatch.Groups["scope"].Value;
            }

            yield return new()
            {
                Name = name,
                ExpectedScope = expectedScope,
            };
        }
    }
}

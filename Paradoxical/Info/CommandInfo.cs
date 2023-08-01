using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Paradoxical.Info;

public partial class CommandInfo
{
    private const string TEXT_FILE = "Paradoxical.Resources.Lists.Commands.txt";

    public string Name { get; init; } = string.Empty;

    public static IEnumerable<CommandInfo> ParseText()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? list = assembly.ReadEmbeddedResource(TEXT_FILE);

        if (list == null)
        {
            throw new Exception($"Did not find embedded text resource '{TEXT_FILE}'!");
        }

        return ParseText(list);
    }

    public static IEnumerable<CommandInfo> ParseText(string text)
    {
        string[] chunks = text.Split(Environment.NewLine, StringSplitOptions.TrimEntries);
        foreach (string chunk in chunks)
        {
            if (chunk.IsEmpty() == true)
            {
                continue;
            }

            yield return new()
            {
                Name = chunk,
            };
        }
    }
}

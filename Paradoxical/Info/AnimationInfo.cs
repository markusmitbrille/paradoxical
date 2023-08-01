using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Paradoxical.Info;

public partial class AnimationInfo
{
    private const string TEXT_FILE = "Paradoxical.Resources.Lists.Animations.txt";

    public string Name { get; init; } = string.Empty;

    public static IEnumerable<AnimationInfo> ParseText()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? list = assembly.ReadEmbeddedResource(TEXT_FILE);

        if (list == null)
        {
            throw new Exception($"Did not find embedded text resource '{TEXT_FILE}'!");
        }

        return ParseText(list);
    }

    public static IEnumerable<AnimationInfo> ParseText(string text)
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

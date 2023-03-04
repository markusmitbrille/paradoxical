using FuzzySharp;
using System.Text.RegularExpressions;

namespace Paradoxical.Core;

public static partial class ParadoxPattern
{
    [GeneratedRegex(@"(?<filter>.+)")]
    private static partial Regex GetFilterRegex();
    public static Regex FilterRegex => GetFilterRegex();

    [GeneratedRegex(@"id:(?<filter>\d+)")]
    private static partial Regex GetIdFilterRegex();
    public static Regex IdFilterRegex => GetIdFilterRegex();

    [GeneratedRegex(@"name:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetNameFilterRegex();
    public static Regex NameFilterRegex => GetNameFilterRegex();

    [GeneratedRegex(@"code:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetCodeFilterRegex();
    public static Regex CodeFilterRegex => GetCodeFilterRegex();

    [GeneratedRegex(@"tooltip:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTooltipFilterRegex();
    public static Regex TooltipFilterRegex => GetTooltipFilterRegex();

    public static bool FuzzyMatch(this Regex regex, string text, string filter, int ratio = 80)
    {
        Match match = regex.Match(filter);

        if (match.Success == false)
        { return false; }

        string target = match.Groups["filter"].Value;

        return Fuzz.PartialRatio(text, target) > ratio;
    }
}

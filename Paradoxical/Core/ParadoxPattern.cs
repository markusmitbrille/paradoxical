using FuzzySharp;
using System.Text.RegularExpressions;

namespace Paradoxical.Core;

public static partial class ParadoxPattern
{
    [GeneratedRegex(@"(?<filter>.+)")]
    private static partial Regex GetFilterRegex();
    public static Regex FilterRegex => GetFilterRegex();

    [GeneratedRegex(@"type:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTypeFilterRegex();
    public static Regex TypeFilterRegex => GetTypeFilterRegex();

    [GeneratedRegex(@"id:(?<filter>\d+)")]
    private static partial Regex GetIdFilterRegex();
    public static Regex IdFilterRegex => GetIdFilterRegex();

    [GeneratedRegex(@"n:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetNameFilterRegex();
    public static Regex NameFilterRegex => GetNameFilterRegex();

    [GeneratedRegex(@"c:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetCodeFilterRegex();
    public static Regex CodeFilterRegex => GetCodeFilterRegex();

    [GeneratedRegex(@"tt:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTooltipFilterRegex();
    public static Regex TooltipFilterRegex => GetTooltipFilterRegex();

    [GeneratedRegex(@"t:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTitleFilterRegex();
    public static Regex TitleFilterRegex => GetTitleFilterRegex();

    [GeneratedRegex(@"d:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetDescriptionFilterRegex();
    public static Regex DescriptionFilterRegex => GetDescriptionFilterRegex();

    /// <summary>
    /// Evaluates whether <paramref name="text"/> meets the conditions defined by <paramref name="filter"/>.
    /// </summary>
    /// <param name="regex">Pattern that extracts the target value from <paramref name="filter"/>.</param>
    /// <param name="text">Text value that is evaluated against a target value extracted from <paramref name="filter"/>.</param>
    /// <param name="filter">Filter text used to determine whether <paramref name="text"/> meets some condition.</param>
    /// <param name="ratio">Fuzziness ratio; more means more exact, less means more fuzzy.</param>
    /// <returns><c>true</c> if <paramref name="text"/> to <paramref name="filter"/> partial ratio is greater than <paramref name="ratio"/>, otherwise <c>false</c>.
    /// <c>Null</c> if <paramref name="filter"/> does not match <paramref name="regex"/>.
    /// </returns>
    public static bool? FuzzyMatch(this Regex regex, string text, string filter, int ratio = 80)
    {
        Match match = regex.Match(filter);

        if (match.Success == false)
        { return null; }

        string target = match.Groups["filter"].Value;

        return Fuzz.PartialRatio(text, target) > ratio;
    }

    /// <summary>
    /// Evaluates whether <paramref name="text"/> meets the conditions defined by <paramref name="filter"/>.
    /// </summary>
    /// <param name="regex">Pattern that extracts the target value from <paramref name="filter"/>.</param>
    /// <param name="text">Text value that is evaluated against a target value extracted from <paramref name="filter"/>.</param>
    /// <param name="filter">Filter text used to determine whether <paramref name="text"/> meets some condition.</param>
    /// <param name="ratio">Fuzziness ratio; more means more exact, less means more fuzzy.</param>
    /// <returns><c>true</c> if <paramref name="text"/> equals <paramref name="filter"/>, otherwise <c>false</c>.
    /// <c>Null</c> if <paramref name="filter"/> does not match <paramref name="regex"/>.
    /// </returns>
    public static bool? ExactMatch(this Regex regex, string text, string filter)
    {
        Match match = regex.Match(filter);

        if (match.Success == false)
        { return null; }

        string target = match.Groups["filter"].Value;

        return text == target;
    }
}

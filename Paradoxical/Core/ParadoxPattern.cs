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

    [GeneratedRegex(@"name:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetNameFilterRegex();
    public static Regex NameFilterRegex => GetNameFilterRegex();

    [GeneratedRegex(@"dir:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetDirFilterRegex();
    public static Regex DirFilterRegex => GetDirFilterRegex();

    [GeneratedRegex(@"file:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetFileFilterRegex();
    public static Regex FileFilterRegex => GetFileFilterRegex();

    [GeneratedRegex(@"code:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetCodeFilterRegex();
    public static Regex CodeFilterRegex => GetCodeFilterRegex();

    [GeneratedRegex(@"tt:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTooltipFilterRegex();
    public static Regex TooltipFilterRegex => GetTooltipFilterRegex();

    [GeneratedRegex(@"ttl:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetTitleFilterRegex();
    public static Regex TitleFilterRegex => GetTitleFilterRegex();

    [GeneratedRegex(@"desc:(?>(?<filter>\w+)|""(?<filter>[^""]*)"")")]
    private static partial Regex GetDescriptionFilterRegex();
    public static Regex DescriptionFilterRegex => GetDescriptionFilterRegex();
}

using Paradoxical.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Paradoxical.Core;

public interface ISearchable
{
    public string Filter { get; }
    public IEnumerable<Tag> Tags { get; }
}

public static partial class Search
{
    [GeneratedRegex(@"(?<filter>.+)")]
    private static partial Regex GetFilterRegex();
    public static Regex FilterRegex => GetFilterRegex();

    [GeneratedRegex(@"(?<tagname>\w+):(?>(?<tagvalue>\w+)|""(?<tagvalue>[^""]*)"")")]
    private static partial Regex GetTagRegex();
    public static Regex TagRegex => GetTagRegex();

    public static bool Filter(this ISearchable searchable, string filter)
    {
        if (filter.IsEmpty() == true)
        { return true; }

        List<Tag> tags = new();

        for (Match match = TagRegex.Match(filter); match.Success == true; match = TagRegex.Match(filter))
        {
            string tagname = match.Groups["tagname"].Value;
            string tagvalue = match.Groups["tagvalue"].Value;

            tags.Add(new(tagname, tagvalue));

            filter = filter.Remove(match.Index, match.Length);
        }

        if (tags.All(searchable.Tags.Contains) == false)
        { return false; }

        filter = filter.Trim();

        if (filter.IsEmpty() == true)
        { return true; }

        if (FilterRegex.FuzzyMatch(searchable.Filter, filter) == false)
        { return false; }

        return true;
    }
}

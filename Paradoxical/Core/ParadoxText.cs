using System.IO;

namespace Paradoxical.Core;

public static class ParadoxText
{
    public static string Indentation => "    ";
    public static string NewParagraph => @"\n";

    public static int IndentLevel { get; set; } = 0;

    public static string Namify(this string name)
    {
        return name
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "_");
    }

    public static TextWriter Indent(this TextWriter writer)
    {
        for (int i = 0; i < IndentLevel; i++)
        {
            writer.Write(Indentation);
        }

        return writer;
    }

    public static void WriteLocLine(this TextWriter writer, string key, string loc)
    {
        // replace quotation marks with escape sequence
        loc = loc.Replace("\"", "\\\"");

        writer.WriteLine($@" {key}:0 ""{loc}""");
    }
}

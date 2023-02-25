using System.IO;

namespace Paradoxical.Model;

public static class ParadoxText
{
    public static string Indentation => "    ";
    public static string NewParagraph => @"\n\n";

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
        writer.WriteLine($@" {key}:0 ""{loc}""");
    }
}

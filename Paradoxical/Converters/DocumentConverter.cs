using Paradoxical.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;

namespace Paradoxical.Converters;

public class DocumentConverter : IValueConverter
{
    private StringBuilder Builder { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string source)
        { return value; }

        var paragraphs = source
            .Split(ParadoxText.NewParagraph)
            .Select(text => new Paragraph(new Run(text)));

        FlowDocument document = new();
        document.Blocks.AddRange(paragraphs);

        return document;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not FlowDocument document)
        { return value; }

        Builder.Clear();

        var paragraphs = document.Blocks
            .Select(block => new TextRange(block.ContentStart, block.ContentEnd).Text);

        string source = string.Join(ParadoxText.NewParagraph, paragraphs);

        return source;
    }
}

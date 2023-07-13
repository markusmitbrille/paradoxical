using Paradoxical.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Paradoxical.Converters;

public class CodeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string text)
        { return value; }

        text = text.Replace(ParadoxText.NewParagraph, "\r\n");

        return text;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string text)
        { return value; }

        text = text.Replace("\r\n", ParadoxText.NewParagraph);

        return text;
    }
}

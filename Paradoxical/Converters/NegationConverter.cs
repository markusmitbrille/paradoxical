using System;
using System.Globalization;
using System.Windows.Data;

namespace Paradoxical.Converters;

public class NegationConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : null;
    }
}

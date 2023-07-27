using System;
using System.Globalization;
using System.Windows.Data;

namespace Paradoxical.Converters;

public class NegationConverter : IValueConverter
{
    public bool ConvertNull { get; set; } = true;

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null && ConvertNull == true)
        {
            return false;
        }

        return value is bool b ? !b : null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null && ConvertNull == true)
        {
            return false;
        }

        return value is bool b ? !b : null;
    }
}

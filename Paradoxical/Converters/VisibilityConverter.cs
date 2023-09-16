using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Paradoxical.Converters;

public class VisibilityConverter : IValueConverter
{
    public bool Collapse { get; set; } = true;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Collapse ? Visibility.Collapsed : Visibility.Hidden;
        }

        if (value is bool b && b == false)
        {
            return Collapse ? Visibility.Collapsed : Visibility.Hidden;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Paradoxical.Converters;

public class TypeVisibilityConverter : IValueConverter
{
    public bool Collapse { get; set; } = true;
    public Type? Type { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (Type == null)
        {
            return Visibility.Visible;
        }

        if (value == null)
        {
            return Collapse ? Visibility.Collapsed : Visibility.Hidden;
        }

        if (value.GetType().IsAssignableTo(Type) == false)
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

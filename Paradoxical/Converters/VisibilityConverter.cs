using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Paradoxical.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public bool Collapse { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null
                ? (Collapse ? Visibility.Collapsed : Visibility.Hidden)
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

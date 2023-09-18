using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Paradoxical.View.Properties;

public static class FocusHelper
{
    public static bool GetFocusOnLoad(DependencyObject obj)
    {
        return (bool)obj.GetValue(FocusOnLoadProperty);
    }

    public static void SetFocusOnLoad(DependencyObject obj, bool value)
    {
        obj.SetValue(FocusOnLoadProperty, value);
    }

    public static readonly DependencyProperty FocusOnLoadProperty =
        DependencyProperty.RegisterAttached("FocusOnLoad", typeof(bool), typeof(FrameworkElement), new PropertyMetadata(false, FocusOnLoad_Changed));

    private static void FocusOnLoad_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
        { return; }

        if (e.NewValue is not bool value)
        { return; }

        if (value == true)
        {
            element.Loaded += Element_Loaded;
        }

        if (value == false)
        {
            element.Loaded -= Element_Loaded;
        }
    }

    private static void Element_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element)
        { return; }

        element.Focus();
    }
}

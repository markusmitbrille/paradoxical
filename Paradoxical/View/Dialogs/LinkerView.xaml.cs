using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class LinkerView : UserControl
{
    public LinkerView()
    {
        InitializeComponent();
    }

    private void ListBoxItemSelectedHandler(object sender, RoutedEventArgs e)
    {
        if (sender is not ListBoxItem item)
        { return; }

        item.BringIntoView();
    }
}

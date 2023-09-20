using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class FinderView : UserControl
{
    public FinderView()
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

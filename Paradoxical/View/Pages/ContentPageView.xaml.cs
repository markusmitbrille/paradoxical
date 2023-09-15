using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class ContentPageView : UserControl
{
    public ContentPageView()
    {
        InitializeComponent();
    }

    private void TreeViewItemSelectedHandler(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeViewItem item)
        { return; }

        item.BringIntoView();
    }
}

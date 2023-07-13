using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class EffectTableView : UserControl
{
    public EffectTableView()
    {
        InitializeComponent();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }

    private void ItemsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }
}

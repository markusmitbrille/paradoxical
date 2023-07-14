using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }

    private void ItemsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }
}

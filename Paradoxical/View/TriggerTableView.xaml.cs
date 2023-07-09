using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class TriggerTableView : UserControl
{
    public TriggerTableView()
    {
        InitializeComponent();
    }

    private void TextBoxGotFocusHandler(object sender, RoutedEventArgs e)
    {
        // commit current edit
        ItemsDataGrid.CommitEdit();

        // commit pending edits
        ItemsDataGrid.CommitEdit();
    }
}

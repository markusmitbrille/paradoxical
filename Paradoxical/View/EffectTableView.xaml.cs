using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class EffectTableView : UserControl
{
    public EffectTableView()
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

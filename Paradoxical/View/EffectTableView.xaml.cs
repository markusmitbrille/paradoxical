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
        datagrid.CommitEdit();

        // commit pending edits
        datagrid.CommitEdit();
    }
}

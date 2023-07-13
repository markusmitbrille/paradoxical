using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class EventDetailsView : UserControl
{
    public EventDetailsView()
    {
        InitializeComponent();
    }

    private void OptionsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // commit current edit
        OptionsDataGrid.CommitEdit();

        // commit pending edits
        OptionsDataGrid.CommitEdit();
    }
}

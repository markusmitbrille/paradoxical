using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Paradoxical.View;

public partial class EventDetailsView : UserControl
{
    public EventDetailsView()
    {
        InitializeComponent();
    }

    private void DataGrid_InitializingNewItem(object? sender, InitializingNewItemEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        { return; }

        dataGrid.BeginningEdit += DataGrid_BeginningEdit;
    }

    private void DataGrid_BeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        if (sender is not DataGrid dataGrid)
        { return; }

        dataGrid.CommitEdit(DataGridEditingUnit.Row, false);
        dataGrid.BeginningEdit -= DataGrid_BeginningEdit;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OptionsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        OptionsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }
}

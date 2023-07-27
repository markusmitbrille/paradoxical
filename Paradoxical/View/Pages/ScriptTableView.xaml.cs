﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class ScriptTableView : UserControl
{
    public ScriptTableView()
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

    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        ItemsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ItemsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }

    private void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        ItemsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }
}

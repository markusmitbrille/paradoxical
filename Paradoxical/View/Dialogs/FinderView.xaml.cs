using System.Windows;
using System.Windows.Controls;

namespace Paradoxical.View;

public partial class FinderView : Window
{
    public FinderView()
    {
        InitializeComponent();

        filter.Focus();
    }
}

using System.Windows.Controls;

namespace Paradoxical.View;

public partial class FinderView : UserControl
{
    public FinderView()
    {
        InitializeComponent();

        filter.Focus();
    }
}

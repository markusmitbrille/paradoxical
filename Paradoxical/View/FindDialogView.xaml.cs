using System.Windows.Controls;

namespace Paradoxical.View
{
    public partial class FindDialogView : UserControl
    {
        public FindDialogView()
        {
            InitializeComponent();

            filter.Focus();
        }
    }
}

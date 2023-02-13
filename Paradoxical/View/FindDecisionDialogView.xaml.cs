using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindDecisionDialogView : UserControl
    {
        public FindDecisionDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

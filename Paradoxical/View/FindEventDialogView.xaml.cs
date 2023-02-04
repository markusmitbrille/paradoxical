using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindEventDialogView : UserControl
    {
        public FindEventDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

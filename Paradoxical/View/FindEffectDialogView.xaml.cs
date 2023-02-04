using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindEffectDialogView : UserControl
    {
        public FindEffectDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

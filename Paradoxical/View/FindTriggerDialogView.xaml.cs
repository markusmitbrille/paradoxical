using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class FindTriggerDialogView : UserControl
    {
        public FindTriggerDialogView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class TriggerPageView : UserControl
    {
        public TriggerPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

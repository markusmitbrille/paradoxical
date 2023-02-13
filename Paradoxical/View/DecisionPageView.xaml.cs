using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class DecisionPageView : UserControl
    {
        public DecisionPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

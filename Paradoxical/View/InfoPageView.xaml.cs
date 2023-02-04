using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class InfoPageView : UserControl
    {
        public InfoPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

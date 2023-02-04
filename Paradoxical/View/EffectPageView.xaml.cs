using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class EffectPageView : UserControl
    {
        public EffectPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

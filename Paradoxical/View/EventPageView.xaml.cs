using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
    public partial class EventPageView : UserControl
    {
        public EventPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }
    }
}

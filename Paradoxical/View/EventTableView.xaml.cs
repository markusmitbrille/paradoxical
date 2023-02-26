using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class EventTableView : UserControl
{
    public EventTableView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}

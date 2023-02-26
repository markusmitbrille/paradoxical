using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class TriggerTableView : UserControl
{
    public TriggerTableView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}

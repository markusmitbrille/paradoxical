using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class DecisionTableView : UserControl
{
    public DecisionTableView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}

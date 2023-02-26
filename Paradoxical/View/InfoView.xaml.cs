using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class InfoView : UserControl
{
    public InfoView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}

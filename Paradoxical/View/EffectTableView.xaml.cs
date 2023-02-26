using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class EffectTableView : UserControl
{
    public EffectTableView()
    {
        InitializeComponent();

        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }
}

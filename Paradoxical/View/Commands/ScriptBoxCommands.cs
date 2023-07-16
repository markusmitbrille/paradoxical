using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Paradoxical.View;

public static class ScriptBoxCommands
{
    public static readonly RoutedUICommand OpenComplete = new("Complete Word", "Open Complete", typeof(ScriptBoxCommands), new()
    {
        new KeyGesture(Key.Space, ModifierKeys.Control)
    });

    public static readonly RoutedUICommand ConfirmComplete = new("Confirm", "Confirm Complete", typeof(ScriptBoxCommands), new()
    {
        new KeyGesture(Key.Escape)
    });

    public static readonly RoutedUICommand CancelComplete = new("Cancel", "Cancel Complete", typeof(ScriptBoxCommands), new()
    {
        new KeyGesture(Key.Tab)
    });
}

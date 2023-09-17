using Paradoxical.ViewModel;
using System.Windows.Input;

namespace Paradoxical.View;

public partial class MainWindow
{
    public MainWindow(IShell shell)
    {
        InitializeComponent();
        DataContext = shell;
    }

    private void DragWindowMouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        { DragMove(); }
    }

    private void MenuButtonClickHandler(object sender, System.Windows.RoutedEventArgs e)
    {
        MenuToggleButton.IsChecked = false;
    }

    private void PopupMenuButtonClickHandler(object sender, System.Windows.RoutedEventArgs e)
    {
        PopupMenu.IsPopupOpen = false;
    }
}

using System.Windows.Input;

namespace Paradoxical.View;

public partial class MainWindow
{
    public const string ROOT_DIALOG_IDENTIFIER = "RootDialog";

    public MainWindow()
    {
        InitializeComponent();
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

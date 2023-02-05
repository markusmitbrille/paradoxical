using System.Windows.Controls;
using System.Windows.Input;

namespace Paradoxical.View
{
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuToggleButton.IsChecked = false;
        }
    }
}

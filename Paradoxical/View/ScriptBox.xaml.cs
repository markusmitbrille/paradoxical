using System.Windows.Input;

namespace Paradoxical.View
{
    /// <summary>
    /// Interaction logic for ScriptBox.xaml
    /// </summary>
    public partial class ScriptBox : Xceed.Wpf.Toolkit.RichTextBox
    {
        public ScriptBox()
        {
            InitializeComponent();
            TextChanged += (sender, e) => Compile();
        }

        private void Compile()
        {
            // TODO: format text via text pointers
        }

        private void NeverExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        private void CompleteWordCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: word completion popup
        }
    }
}

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Paradoxical.View
{
    /// <summary>
    /// Interaction logic for AboutPageView.xaml
    /// </summary>
    public partial class AboutPageView : UserControl
    {
        public AboutPageView()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        private void HyperlinkRequestNavigateHandler(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
                Verb = "open"
            });

            e.Handled = true;
        }
    }
}

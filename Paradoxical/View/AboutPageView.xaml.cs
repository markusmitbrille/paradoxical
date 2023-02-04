using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Paradoxical.View;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true,
            Verb = "open"
        });

        e.Handled = true;
    }
}

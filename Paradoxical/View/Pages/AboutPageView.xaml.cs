using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Paradoxical.View;

public partial class AboutPageView : UserControl
{
    public AboutPageView()
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

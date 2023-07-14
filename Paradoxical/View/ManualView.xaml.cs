using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Paradoxical.View;

public partial class ManualView : UserControl
{
    public ManualView()
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

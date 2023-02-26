using Paradoxical.Core;

namespace Paradoxical.ViewModel;

public class AboutViewModel : PageViewModelBase
{
    public override string PageName => "About";

    public AboutViewModel(NavigationViewModel navigation)
        : base(navigation)
    {
    }
}
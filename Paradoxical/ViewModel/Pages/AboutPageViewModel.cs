using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class AboutPageViewModel : PageViewModel
{
    public override string PageName => "About";

    public AboutPageViewModel(IShell shell, IMediatorService mediator)
        : base(shell, mediator)
    {
    }
}

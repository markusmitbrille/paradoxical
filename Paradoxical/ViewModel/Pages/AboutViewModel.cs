using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class AboutViewModel : PageViewModel
{
    public override string PageName => "About";

    public AboutViewModel(IShell shell, IMediatorService mediator)
        : base(shell, mediator)
    {
    }
}

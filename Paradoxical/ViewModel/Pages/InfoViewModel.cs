using Paradoxical.Core;
using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class InfoViewModel : PageViewModel
{
    public override string PageName => "Mod Info";

    public IDataService Data { get; }

    public InfoViewModel(NavigationViewModel navigation, IDataService data)
        : base(navigation)
    {
        Data = data;
    }
}
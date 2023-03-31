using Paradoxical.Core;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class OptionDetailsViewModel : PageViewModel
{
    public override string PageName => "Option Details";

    public IOptionService Service { get; }

    public OptionDetailsViewModel(NavigationViewModel navigation, IOptionService service)
        : base(navigation)
    {
        Service = service;
    }
}
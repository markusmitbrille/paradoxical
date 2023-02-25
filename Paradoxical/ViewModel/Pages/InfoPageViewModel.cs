using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public partial class InfoPageViewModel : PageViewModelBase
{
    public override string PageName => "Mod Info";

    public ApplicationViewModel App { get; }
    public IDataService Data { get; }

    public InfoPageViewModel(ApplicationViewModel app, IDataService data)
    {
        App = app;
        Data = data;
    }
}
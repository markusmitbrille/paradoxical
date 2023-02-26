using Paradoxical.Services;

namespace Paradoxical.ViewModel;

public class InfoViewModel : PageViewModelBase
{
    public override string PageName => "Mod Info";

    public ApplicationViewModel App { get; }
    public IDataService Data { get; }

    public InfoViewModel(ApplicationViewModel app, IDataService data)
    {
        App = app;
        Data = data;
    }
}
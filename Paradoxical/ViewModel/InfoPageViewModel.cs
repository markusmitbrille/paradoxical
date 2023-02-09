using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModelBase
    {
        public override string PageName => "Mod Info";

        public Context Context { get; }

        public InfoPageViewModel(Context context)
        {
            Context = context;
        }
    }
}
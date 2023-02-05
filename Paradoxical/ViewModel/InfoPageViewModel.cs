using Paradoxical.Data;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModelBase
    {
        public override string PageName => "Mod Info";

        public ModContext Context { get; }

        public InfoPageViewModel(ModContext context)
        {
            Context = context;
        }
    }
}
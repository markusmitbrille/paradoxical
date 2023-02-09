using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModelBase
    {
        public override string PageName => "Mod Info";

        public Context CurrentContext => Context.Current;

        public InfoPageViewModel()
        {
        }
    }
}
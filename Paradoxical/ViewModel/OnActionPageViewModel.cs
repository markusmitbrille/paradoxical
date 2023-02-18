using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class OnActionPageViewModel : PageViewModelBase
    {
        public override string PageName => "On-Actions";

        public Context CurrentContext => Context.Current;

        public OnActionPageViewModel()
        {
        }
    }
}
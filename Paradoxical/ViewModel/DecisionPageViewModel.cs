using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class DecisionPageViewModel : PageViewModelBase
    {
        public override string PageName => "Decisions";

        public Context CurrentContext => Context.Current;

        public DecisionPageViewModel()
        {
        }
    }
}
using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class TriggerPageViewModel : PageViewModelBase
    {
        public override string PageName => "Triggers";

        public Context CurrentContext => Context.Current;

        public TriggerPageViewModel()
        {
        }
    }
}
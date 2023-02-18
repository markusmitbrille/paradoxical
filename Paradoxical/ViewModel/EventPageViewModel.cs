using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class EventPageViewModel : PageViewModelBase
    {
        public override string PageName => "Events";

        public Context CurrentContext => Context.Current;

        public EventPageViewModel()
        {
        }
    }
}
using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class EffectPageViewModel : PageViewModelBase
    {
        public override string PageName => "Effects";

        public Context CurrentContext => Context.Current;

        public EffectPageViewModel()
        {
        }
    }
}
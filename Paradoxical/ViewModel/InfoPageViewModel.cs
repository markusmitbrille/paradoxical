using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModel
    {
        public override string PageName => "Mod Info";

        public ModContext Context { get; }

        public InfoPageViewModel(ModContext context)
        {
            Context = context;
        }
    }
}
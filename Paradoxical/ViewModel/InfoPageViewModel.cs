using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModel
    {
        public override string PageName => "Mod Info";

        [ObservableProperty]
        private ParadoxMod? activeMod;
    }
}
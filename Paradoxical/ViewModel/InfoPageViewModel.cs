using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel
{
    public partial class InfoPageViewModel : PageViewModel
    {
        public override string PageName => "Mod Info";

        [ObservableProperty]
        private ParadoxModViewModel? activeMod;
    }
}
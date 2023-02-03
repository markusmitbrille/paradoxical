using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxEffectViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string code = "";
    }
}
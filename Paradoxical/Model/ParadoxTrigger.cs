using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.Model
{
    public partial class ParadoxTrigger : ObservableObject
    {
        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";
    }
}
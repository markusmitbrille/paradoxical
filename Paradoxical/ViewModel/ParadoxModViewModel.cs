using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxModViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string version = "";
        [ObservableProperty]
        private string gameVersion = "";
        [ObservableProperty]
        private string eventNamespace = "";

        public ObservableCollection<ParadoxEventViewModel> Events { get; } = new();
        public ObservableCollection<ParadoxTriggerViewModel> Triggers { get; } = new();
        public ObservableCollection<ParadoxEffectViewModel> Effects { get; } = new();
    }
}

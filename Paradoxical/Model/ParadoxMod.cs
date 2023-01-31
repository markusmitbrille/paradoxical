using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.Model
{
    public partial class ParadoxMod : ObservableObject
    {
        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string version = "";
        [ObservableProperty]
        private string gameVersion = "";
        [ObservableProperty]
        private string eventNamespace = "";

        public ObservableCollection<ParadoxEvent> Events { get; } = new();
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();
        public ObservableCollection<ParadoxEffect> Effects { get; } = new();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.Model
{
    public partial class ParadoxEvent : ObservableObject
    {
        [ObservableProperty]
        private int id = 0;

        [ObservableProperty]
        private string title = "";
        [ObservableProperty]
        private string description = "";
        [ObservableProperty]
        private string theme = "";
        [ObservableProperty]
        private string background = "";
        [ObservableProperty]
        private string sound = "";

        public ObservableCollection<ParadoxEventOption> Options { get; } = new();

        [ObservableProperty]
        private ParadoxPortrait leftPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait rightPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerLeftPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerRightPortrait = new();
        [ObservableProperty]
        private ParadoxPortrait lowerCenterPortrait = new();

        [ObservableProperty]
        private string trigger = "";
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();

        [ObservableProperty]
        private string immediateEffect = "";
        public ObservableCollection<ParadoxEffect> ImmediateEffects { get; } = new();

        [ObservableProperty]
        private string afterEffect = "";
        public ObservableCollection<ParadoxEffect> AfterEffects { get; } = new();
    }
}

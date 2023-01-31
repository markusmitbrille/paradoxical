using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Paradoxical.Model
{
    public partial class ParadoxPortrait : ObservableObject
    {
        [ObservableProperty]
        private string character = "";
        [ObservableProperty]
        private string animation = "";
        [ObservableProperty]
        private string outfitTags = "";
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxPortraitViewModel : ObservableObject
    {
        [ObservableProperty]
        private string character = "";
        [ObservableProperty]
        private string animation = "";
        [ObservableProperty]
        private string outfitTags = "";
    }
}
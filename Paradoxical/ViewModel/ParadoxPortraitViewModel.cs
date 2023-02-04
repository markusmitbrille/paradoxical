using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System.Collections.Generic;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxPortraitViewModel : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string character = "";
        [ObservableProperty]
        private string animation = "";
        [ObservableProperty]
        private string outfitTags = "";

        public ParadoxPortraitViewModel(ModContext context)
        {
            Context = context;
        }
    }
}
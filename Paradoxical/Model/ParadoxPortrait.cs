using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.Model
{
    public partial class ParadoxPortrait : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string character = "";
        [ObservableProperty]
        private string animation = "";
        [ObservableProperty]
        private string outfitTags = "";

        public ParadoxPortrait(ModContext context)
        {
            Context = context;
        }

        public ParadoxPortrait(ModContext context, ParadoxPortrait other) : this(context)
        {
            character = other.character;
            animation = other.animation;
            outfitTags = other.outfitTags;
        }
    }
}
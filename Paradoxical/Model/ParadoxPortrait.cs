using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxPortrait : ObservableObject
    {
        public Context Context { get; }

        [ObservableProperty]
        private string character = "";
        [ObservableProperty]
        private string animation = "";
        [ObservableProperty]
        private string outfitTags = "";

        public ParadoxPortrait(Context context)
        {
            Context = context;
        }

        public ParadoxPortrait(Context context, ParadoxPortrait other) : this(context)
        {
            character = other.character;
            animation = other.animation;
            outfitTags = other.outfitTags;
        }

        internal void Write(TextWriter writer)
        {
            if (Character == string.Empty)
            {
                writer.Indent().WriteLine("character = ROOT");
            }
            else
            {
                writer.Indent().WriteLine($"character = {Character}");
            }

            if (Animation != string.Empty)
            {
                writer.Indent().WriteLine($"animation = {Animation}");
            }

            if (OutfitTags != string.Empty)
            {
                writer.Indent().WriteLine($"outfit_tags = {{ {OutfitTags} }}");
            }
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.Model
{
    public partial class ParadoxEffect : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string code = "";

        public ParadoxEffect(ModContext context)
        {
            Context = context;
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.Model
{
    public partial class ParadoxMod : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string version = "";
        [ObservableProperty]
        private string gameVersion = "";
        [ObservableProperty]
        private string eventNamespace = "";

        public ParadoxMod(ModContext context)
        {
            Context = context;
        }
    }
}

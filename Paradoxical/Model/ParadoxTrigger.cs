using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.Model
{
    public partial class ParadoxTrigger : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";

        public ParadoxTrigger(ModContext context)
        {
            Context = context;
        }
    }
}
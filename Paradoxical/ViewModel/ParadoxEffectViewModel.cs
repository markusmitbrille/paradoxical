using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxEffectViewModel : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string code = "";

        public ParadoxEffectViewModel(ModContext context)
        {
            Context = context;
        }
    }
}
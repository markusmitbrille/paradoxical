using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxTriggerViewModel : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";
        
        public ParadoxTriggerViewModel(ModContext context)
        {
            Context = context;
        }
    }
}
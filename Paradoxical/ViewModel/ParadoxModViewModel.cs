using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Paradoxical.ViewModel
{
    public partial class ParadoxModViewModel : ObservableObject
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

        public ParadoxModViewModel(ModContext context)
        {
            Context = context;
        }
    }
}

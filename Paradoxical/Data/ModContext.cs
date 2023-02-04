using Paradoxical.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxical.Data
{
    public class ModContext
    {
        public ParadoxModViewModel Info { get; }

        public ObservableCollection<ParadoxEventViewModel> Events { get; } = new();
        public ObservableCollection<ParadoxTriggerViewModel> Triggers { get; } = new();
        public ObservableCollection<ParadoxEffectViewModel> Effects { get; } = new();

        public ModContext()
        {
            Info = new(this);
        }
    }
}

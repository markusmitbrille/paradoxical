using Paradoxical.Model;
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
        public ParadoxMod Info { get; }

        public ObservableCollection<ParadoxEvent> Events { get; } = new();
        public ObservableCollection<ParadoxTrigger> Triggers { get; } = new();
        public ObservableCollection<ParadoxEffect> Effects { get; } = new();

        public ModContext()
        {
            Info = new(this);
        }
    }
}

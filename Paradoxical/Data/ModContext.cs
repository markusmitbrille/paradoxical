using Paradoxical.Model;
using System.Collections.ObjectModel;

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

using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System;

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

            name = $"Trigger [{Guid.NewGuid().ToString()[0..4]}]";
        }

        public ParadoxTrigger(ModContext context, ParadoxTrigger other) : this(context)
        {
            name = other.name;
            code = other.name;
        }
    }
}
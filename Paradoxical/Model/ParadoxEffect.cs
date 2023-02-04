using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System;

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

            name = $"Effect [{Guid.NewGuid().ToString()[0..4]}]";
        }

        public ParadoxEffect(ModContext context, ParadoxEffect other) : this(context)
        {
            name = other.name;
            code = other.name;
        }
    }
}
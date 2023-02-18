using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Paradoxical.Model
{
    public abstract partial class ParadoxElement : ObservableObject
    {
        [ObservableProperty]
        private string name;

        public ParadoxElement()
        {
            name = $"element_{Guid.NewGuid().ToString()[0..4]}";
        }

        public abstract void Delete();
    }
}

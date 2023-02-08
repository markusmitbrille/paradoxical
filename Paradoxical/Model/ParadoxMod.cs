using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxMod : ObservableObject
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

        public ParadoxMod(ModContext context)
        {
            Context = context;
        }

        public void Write(TextWriter writer, string dir, string file)
        {
            writer.WriteLine(
$@"version=""{Version}""
tags={{
    ""Events""
}}
name=""{Name}""
supported_version=""{GameVersion}""
path=""mod/{file}.zip"""
                );
        }
    }
}

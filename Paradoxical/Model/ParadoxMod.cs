﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxMod : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string version = "";
        [ObservableProperty]
        private string gameVersion = "";
        [ObservableProperty]
        private string eventNamespace = "";

        public ParadoxMod()
        {
            name = "My Mod";
            version = "1.0";
            gameVersion = "1.8.*";
            eventNamespace = "mymod";
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
path=""mod/{file}"""
                );
        }
    }
}

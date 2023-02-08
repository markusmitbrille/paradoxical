using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System;
using System.IO;

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

            name = $"Trigger {Guid.NewGuid().ToString()[0..4]}";
        }

        public ParadoxTrigger(ModContext context, ParadoxTrigger other) : this(context)
        {
            name = other.name;
            code = other.name;
        }

        public void Write(TextWriter writer)
        {
            string prefix = Context.Info.EventNamespace.Namify();

            writer.Indent().WriteLine($"{prefix}_{Name.Namify()} = {{");
            ParadoxText.IndentLevel++;

            foreach (string line in Code.Split(Environment.NewLine))
            {
                writer.Indent().WriteLine(line);
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }
}
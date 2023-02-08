using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxEffect : ObservableObject
    {
        public ModContext Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";
        [ObservableProperty]
        private string tooltip = "";

        public ParadoxEffect(ModContext context)
        {
            Context = context;

            name = $"Effect {Guid.NewGuid().ToString()[0..4]}";
        }

        public ParadoxEffect(ModContext context, ParadoxEffect other) : this(context)
        {
            name = other.name;
            code = other.name;
        }

        public void Write(TextWriter writer)
        {
            string prefix = Context.Info.EventNamespace.Namify();

            writer.Indent().WriteLine($"{prefix}_{Name.Namify()} = {{");
            ParadoxText.IndentLevel++;

            if (Tooltip != string.Empty)
            {
                writer.Indent().WriteLine($"custom_tooltip = {prefix}.{Name.Namify()}.tt");

                writer.Indent().WriteLine("hidden_effect = {");
                ParadoxText.IndentLevel++;
            }

            foreach (string line in Code.Split(Environment.NewLine))
            {
                writer.Indent().WriteLine(line);
            }

            if (Tooltip != string.Empty)
            {
                ParadoxText.IndentLevel--;
                writer.Indent().WriteLine("}");
            }

            ParadoxText.IndentLevel--;
            writer.Indent().WriteLine("}");
        }
    }
}

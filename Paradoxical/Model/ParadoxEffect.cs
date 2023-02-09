using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Data;
using System;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxEffect : ObservableObject
    {
        public Context Context { get; }

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";
        [ObservableProperty]
        private string tooltip = "";

        public ParadoxEffect(Context context)
        {
            Context = context;

            name = $"Effect_{Guid.NewGuid().ToString()[0..4]}";
            code = "# some effect";
        }

        public ParadoxEffect(Context context, ParadoxEffect other) : this(context)
        {
            name = other.name;
            code = other.name;
        }

        public void Write(TextWriter writer)
        {
            writer.Indent().WriteLine($"{Context.Info.EventNamespace}_{Name} = {{");
            ParadoxText.IndentLevel++;

            if (Tooltip != string.Empty)
            {
                writer.Indent().WriteLine("custom_tooltip = {");
                ParadoxText.IndentLevel++;

                writer.Indent().WriteLine($"text = {Context.Info.EventNamespace}.{Name}.tt");
                writer.WriteLine();
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

        public void WriteLoc(TextWriter writer)
        {
            writer.WriteLocLine($"{Context.Info.EventNamespace}.{Name}.tt", Tooltip);
        }
    }
}

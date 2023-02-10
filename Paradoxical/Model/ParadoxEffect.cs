using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxEffect : ObservableObject
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string name = "";
        [ObservableProperty]
        private string code = "";
        [ObservableProperty]
        private string tooltip = "";

        public ParadoxEffect()
        {
            name = $"Effect_{Guid.NewGuid().ToString()[0..4]}";
            code = "# some effect";
        }

        public ParadoxEffect(ParadoxEffect other) : this()
        {
            name = other.name;
            code = other.name;
            tooltip = other.tooltip;
        }

        public void Write(TextWriter writer)
        {
            writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{Name} = {{");
            ParadoxText.IndentLevel++;

            if (Tooltip != string.Empty)
            {
                writer.Indent().WriteLine("custom_tooltip = {");
                ParadoxText.IndentLevel++;

                writer.Indent().WriteLine($"text = {Context.Current.Info.EventNamespace}.{Name}.tt");
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
            writer.WriteLocLine($"{Context.Current.Info.EventNamespace}.{Name}.tt", Tooltip);
        }
    }
}

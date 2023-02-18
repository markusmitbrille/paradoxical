using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxEffect : ParadoxElement
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string code = "";
        [ObservableProperty]
        private string tooltip = "";
        [ObservableProperty]
        private bool hidden = false;

        public ParadoxEffect() : base()
        {
            Context.Current.Effects.Add(this);

            code = "# some effect";
        }

        public ParadoxEffect(ParadoxEffect other) : this()
        {
            code = other.code;
            tooltip = other.tooltip;
        }

        public override void Delete()
        {
            foreach (ParadoxEvent evt in Context.Current.Events)
            {
                evt.ImmediateEffects.Remove(this);
                evt.AfterEffects.Remove(this);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Effects.Remove(this);
                }
            }

            foreach (ParadoxDecision dec in Context.Current.Decisions)
            {
                dec.Effects.Remove(this);
            }

            foreach (ParadoxOnAction act in Context.Current.OnActions)
            {
                act.Effects.Remove(this);
            }

            Context.Current.Effects.Remove(this);
        }

        public void Write(TextWriter writer)
        {
            writer.Indent().WriteLine($"{Context.Current.Info.EventNamespace}_{Name} = {{");
            ParadoxText.IndentLevel++;

            if (Hidden == true)
            {
                writer.Indent().WriteLine("hidden_effect = {");
                ParadoxText.IndentLevel++;
            }
            else if (Tooltip != string.Empty)
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

            if (Hidden == true)
            {
                ParadoxText.IndentLevel--;
                writer.Indent().WriteLine("}");
            }
            else if (Tooltip != string.Empty)
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

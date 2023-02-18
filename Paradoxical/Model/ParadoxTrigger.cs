using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace Paradoxical.Model
{
    public partial class ParadoxTrigger : ParadoxElement
    {
        public Context CurrentContext => Context.Current;

        [ObservableProperty]
        private string code = "";
        [ObservableProperty]
        private string tooltip = "";

        public ParadoxTrigger() : base()
        {
            Context.Current.Triggers.Add(this);

            code = "# some trigger";
        }

        public ParadoxTrigger(ParadoxTrigger other) : this()
        {
            code = other.code;
            tooltip = other.tooltip;
        }

        public override void Delete()
        {
            foreach (ParadoxEvent evt in Context.Current.Events)
            {
                evt.Triggers.Remove(this);

                foreach (ParadoxEventOption opt in evt.Options)
                {
                    opt.Triggers.Remove(this);
                }
            }

            foreach (ParadoxDecision dec in Context.Current.Decisions)
            {
                dec.IsShownTriggers.Remove(this);
                dec.IsValidTriggers.Remove(this);
                dec.IsValidFailureTriggers.Remove(this);
            }

            foreach (ParadoxOnAction act in Context.Current.OnActions)
            {
                act.Triggers.Remove(this);
            }

            Context.Current.Triggers.Remove(this);
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
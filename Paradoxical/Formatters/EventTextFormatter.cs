using Paradoxical.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit;

namespace Paradoxical.Formatters
{
    public class EventTextFormatter : ITextFormatter
    {
        private StringBuilder builder = new();

        public string GetText(FlowDocument document)
        {
            builder.Clear();

            foreach (var block in document.Blocks)
            {
                builder.Append(new TextRange(block.ContentStart, block.ContentEnd).Text);
                builder.Append(ParadoxText.NewParagraph);
            }

            if (builder.Length >= ParadoxText.NewParagraph.Length)
            {
                builder.Length -= ParadoxText.NewParagraph.Length;
            }

            return builder.ToString();
        }

        public void SetText(FlowDocument document, string text)
        {

            List<Paragraph> paragraphs = new();
            foreach (var pText in text.Split(ParadoxText.NewParagraph))
            {
                paragraphs.Add(new Paragraph(new Run(pText)));
            }

            document.Blocks.Clear();
            document.Blocks.AddRange(paragraphs);
        }
    }
}

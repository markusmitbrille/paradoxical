using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit;

namespace Paradoxical.Formatters
{
    public class CodeFormatter : ITextFormatter
    {
        private readonly StringBuilder builder = new();

        public string GetText(FlowDocument document)
        {
            builder.Clear();

            foreach (var block in document.Blocks)
            {
                builder.Append(new TextRange(block.ContentStart, block.ContentEnd).Text);
                builder.Append(Environment.NewLine);
            }

            if (builder.Length >= Environment.NewLine.Length)
            {
                builder.Length -= Environment.NewLine.Length;
            }

            return builder.ToString();
        }

        public void SetText(FlowDocument document, string text)
        {
            List<Paragraph> paragraphs = new();
            foreach (var pText in text.Split(Environment.NewLine))
            {
                paragraphs.Add(new Paragraph(new Run(pText)));
            }

            document.Blocks.Clear();
            document.Blocks.AddRange(paragraphs);
        }
    }
}

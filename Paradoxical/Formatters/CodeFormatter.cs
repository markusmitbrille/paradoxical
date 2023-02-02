using System.Text;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit;

namespace Paradoxical.Formatters
{
    public class CodeFormatter : ITextFormatter
    {
        private StringBuilder builder = new();

        public string GetText(FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        public void SetText(FlowDocument document, string text)
        {
            new TextRange(document.ContentStart, document.ContentEnd).Text = text;
        }
    }
}

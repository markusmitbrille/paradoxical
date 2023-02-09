using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules
{
    public class AsciiRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string text && text.All(char.IsAscii) == false)
            {
                return new ValidationResult(false, "Needs to be an ASCII string.");
            }

            return ValidationResult.ValidResult;
        }
    }
}

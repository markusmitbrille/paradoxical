using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules
{
    public class AlphaNumericRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string text && text.All(char.IsLetterOrDigit) == false)
            {
                return new ValidationResult(false, "Needs to be an alphanumeric string.");
            }

            return ValidationResult.ValidResult;
        }
    }
}

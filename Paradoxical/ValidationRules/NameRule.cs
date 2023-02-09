using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules
{
    public partial class NameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return ValidationResult.ValidResult;
            }

            if (value is not string text)
            {
                return new ValidationResult(false, "Needs to be a string.");
            }

            if (GetNameRegex().IsMatch(text) == false)
            {
                return new ValidationResult(false, "Needs to be a valid element name.");
            }

            return ValidationResult.ValidResult;
        }

        [GeneratedRegex(@"^[a-zA-Z]\w+$")]
        private static partial Regex GetNameRegex();
    }
}

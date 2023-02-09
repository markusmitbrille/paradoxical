using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules
{
    public partial class VersionRule : ValidationRule
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

            if (GetVersionNumberRegex().IsMatch(text) == false)
            {
                return new ValidationResult(false, "Needs to be a version number.");
            }

            return ValidationResult.ValidResult;
        }

        [GeneratedRegex(@"^\d+\.(?>\d+|\*)(?>\.(?>\d+|\*))?$")]
        private static partial Regex GetVersionNumberRegex();
    }
}

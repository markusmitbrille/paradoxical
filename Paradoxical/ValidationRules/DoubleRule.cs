using System.Globalization;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public class DoubleRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string text && double.TryParse(text, out _) == false)
        {
            return new ValidationResult(false, "Needs to be a number.");
        }

        return ValidationResult.ValidResult;
    }
}
